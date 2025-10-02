using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class TimeLoopManager : MonoBehaviour
{
    public static TimeLoopManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Volume postProcessVolume; // ポストプロセスボリューム
    public GameObject ghostPrefab; // ゴーストのプレハブ
    public PlayerRecorder playerRecorder; // プレイヤーの行動を記録するコンポーネント
    public Transform player; // プレイヤーのTransform
    public FlashEffect flashEffect; // フラッシュエフェクト
    public float loopTime = 15000.0f; // タイムループの時間（現在使用されていない可能性がありますが保持）

    [Header("Audio Settings")]
    [SerializeField] private AudioSource loopAudioSource; // ループサウンド用のAudioSource
    [SerializeField] private AudioClip monochromeLoopClip; // モノクロ状態時のループサウンドクリップ

    private List<List<PlayerRecorder.FrameData>> allRecordings = new(); // 全ての録画データを保存するリスト
    private List<GameObject> spawnedGhosts = new(); // 生成されたゴーストのゲームオブジェクトリスト
    public Vector3 checkpointPosition; // チェックポイントの位置
    private Quaternion checkpointRotation; // チェックポイントの回転
    private bool isPaused = false; // 現在、タイムループが一時停止（モノクロ）状態かどうか
    private bool isReadyToRecord = false; // 新しいタイムループの録画準備ができているか（チェックポイント通過後）

    // タイムループがアクティブかどうかを示すフラグ
    public bool isTimeLoopActive { get; private set; } = false;

    // 現在のタイムループが開始した絶対時刻
    private float currentLoopStartTime;

    // 現在のタイムループが開始した絶対時刻を取得するメソッド (時間記録は不要になりましたが、互換性のため残します)
    public float GetCurrentLoopStartTime() => currentLoopStartTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 必要であればシーンを跨いで維持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Qキー (またはJoystickButton3) が押されたかを確認
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            // もし現在モノクロ状態（isPaused = true）なら、解除する
            if (isPaused)
            {
                ResumeTimeLoop(); // モノクロ解除とタイムループ再開
            }
            // モノクロ状態ではない（isPaused = false）なら、モノクロ化を試みる
            else
            {
                if (isReadyToRecord && playerRecorder.HasRecording())
                {
                    StartCoroutine(HandleTimeLoop()); // モノクロ化とリプレイ開始
                }
                else if (!isReadyToRecord)
                {
                    Debug.Log("録画準備ができていません：チェックポイントを通過してください。");
                }
                else // playerRecorder.HasRecording() が false の場合
                {
                    Debug.Log("録画できません：録画データがありません。");
                }
            }
        }

        // デバッグ用: 特定のキーでゴーストを消去
        if (Input.GetKeyDown(KeyCode.K)) // 例としてKキーを使用
        {
            Debug.Log("[TimeLoopManager] デバッグ: Kキーが押されました。全てのゴーストを消去します。");
            DestroyAllGhosts(); // 全てのゴーストを破壊するメソッドを呼び出し
        }
    }

    // タイムループを一時停止状態から解除し、モノクロ効果を無効にする。
    void ResumeTimeLoop()
    {
        SetMonochrome(false); // モノクロ効果を解除
        isPaused = false;     // 状態を通常に戻す
        isTimeLoopActive = false; // タイムループ非アクティブに設定
        RestartTimeLoop();    // 時間の進行を再開
    }

    // ゴーストを生成する（現在はReplayAllGhostsから呼ばれる）
    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab);
        ghost.transform.position = Vector3.zero; // 初期位置をリセット
    }

    // チェックポイントを通過したときに呼ばれる処理。
    // プレイヤーの録画を開始し、タイムループの準備を整える。
    public void NotifyCheckpointPassed()
    {
        playerRecorder.StartRecordingFromCheckpoint(); // チェックポイントからの録画を開始
        isReadyToRecord = true; // タイムループの録画準備完了

        // 現在のプレイヤーの位置と回転をチェックポイントとして保存
        Transform playerRoot = playerRecorder.transform;
        checkpointPosition = playerRoot.position;
        checkpointRotation = playerRoot.rotation;

        Debug.Log("チェックポイント通過 → 録画準備完了");
    }

    // 全ての録画データに基づいてゴーストをリプレイする。
    void ReplayAllGhosts()
    {
        float sharedStartTime = Time.time; // ゴーストが同時にリプレイを開始するための共有開始時間

        foreach (var recording in allRecordings)
        {
            SpawnGhost(recording, sharedStartTime); // 各録画データでゴーストを生成
        }
    }

    // 特定の録画データと開始時間に基づいてゴーストを生成し、リプレイを開始する。
    void SpawnGhost(List<PlayerRecorder.FrameData> frames, float sharedStartTime)
    {
        if (frames == null || frames.Count == 0) return;

        GameObject ghost = Instantiate(ghostPrefab); // ゴーストを生成
        var replayer = ghost.GetComponent<GhostReplayer>(); // GhostReplayerコンポーネントを取得

        if (replayer != null)
        {
            replayer.PlayRecording(frames, sharedStartTime); // 録画を再生
            spawnedGhosts.Add(ghost); // 生成されたゴーストのリストに追加
        }
        else
        {
            Debug.LogError("GhostReplayer コンポーネントが見つかりません！"); // エラーログ
            Destroy(ghost); // コンポーネントがなければゴーストを破棄
        }
    }

    // シーン内の全てのゴーストを破壊する。 (アクセス修飾子をpublicに変更)
    public void DestroyAllGhosts()
    {
        foreach (GameObject ghost in spawnedGhosts)
        {
            if (ghost != null)
                Destroy(ghost); // ゴーストを破棄
        }
        spawnedGhosts.Clear(); // リストをクリア
        Debug.Log("[TimeLoopManager] 全てのゴーストが破壊されました。"); // 追加
    }

    // タイムループを再開する（時間や植物の成長など）。
    public void RestartTimeLoop()
    {
        // VineTimeManager が存在するか確認し、タイマーを開始
        if (VineTimeManager.Instance != null)
        {
            VineTimeManager.Instance.StartTimer();
        }
        else
        {
            Debug.LogWarning("VineTimeManager.Instance が見つかりません。");
        }

        // シーン内の全てのSeedling（苗木）をリセットし、成長をスケジュール
        foreach (var seedling in FindObjectsOfType<Seedling>())
        {
            seedling.ResetVines();
            // VineTimeManager.Instance が存在する場合のみスケジュールを試みる
            if (VineTimeManager.Instance != null)
            {
                seedling.ScheduleAutoGrow(VineTimeManager.Instance.GetStartTime());
            }
        }
        // ここにゲーム要素のリセット処理を追加
        ResetGameElementsForLoop();
    }

    // タイムループのメイン処理（コルーチン）。
    // プレイヤーの録画、ゴーストのリプレイ、プレイヤー位置のリセット、モノクロ化などを行う。
    IEnumerator HandleTimeLoop()
    {
        isReadyToRecord = false; // タイムループ起動中は新しい録画準備を一時停止
        isTimeLoopActive = true; // タイムループアクティブに設定
        currentLoopStartTime = Time.time; // 現在のループ開始時刻を記録
        Debug.Log($"[TimeLoopManager] HandleTimeLoop が開始されました。currentLoopStartTime: {currentLoopStartTime:F2}"); // ★追加ログ★

        // ★追加点★ タイムループ開始時にインベントリの松明の数量を保存
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInitialTorchQuantity();
        }

        var frames = playerRecorder.GetFramesSinceCheckpoint(); // チェックポイントからのフレームデータを取得
        if (frames.Count > 0)
        {
            // タイムスタンプを最初のフレームからの相対時間にする
            float firstTime = frames[0].timeStamp;
            for (int i = 0; i < frames.Count; i++)
                frames[i].timeStamp -= firstTime;

            allRecordings.Add(frames); // 録画データをリストに追加
            DestroyAllGhosts();        // 既存のゴーストを全て破壊
            ReplayAllGhosts();         // ゴーストのリプレイを開始

            // フラッシュエフェクトを再生し、完了を待つ。
            // フラッシュ完了後に、プレイヤーリセットやモノクロ化を行う。
            yield return flashEffect.DoFlash(() =>
            {
                // プレイヤーをチェックポイントに戻す
                Transform root = playerRecorder.transform;
                var controller = root.GetComponent<CharacterController>();
                if (controller != null) controller.enabled = false; // CharacterControllerを一時無効にして位置設定

                Rigidbody rb = root.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;     // 速度をリセット
                    rb.angularVelocity = Vector3.zero; // 角速度をリセット
                    rb.position = checkpointPosition; // 位置をチェックポイントに設定
                    rb.rotation = checkpointRotation; // 回転をチェックポイントに設定
                }
                else
                {
                    root.position = checkpointPosition; // Rigidbodyがない場合はTransformを直接設定
                    root.rotation = checkpointRotation;
                }

                if (controller != null) controller.enabled = true; // CharacterControllerを再度有効にする

                // 一時停止状態にする: モノクロ効果適用 + プレイヤー入力無効化
                SetMonochrome(true); // モノクロ効果を有効化し、プレイヤー入力を無効化
                isPaused = true;     // タイムループを一時停止状態に設定
            });

            // HandleTimeLoopが完了し、モノクロ状態の場合（Qで解除待機）は、
            // isReadyToRecordはここではtrueに戻しません。モノクロ解除時に行われます。
        }
        else
        {
            // 録画データがないのにコルーチンが呼ばれてしまった場合のリカバリ
            // (Update()でのチェックがあるため通常は発生しにくいですが念のため)
            Debug.Log("録画データがないためタイムループは実行されませんでした。録画準備状態に戻します。");
            isReadyToRecord = true; // 録画準備状態に戻す
            isTimeLoopActive = false; // タイムループ非アクティブに設定
        }
    }

    // シーンのモノクロ効果を切り替え、ゴーストやプレイヤーの動作を制御する。
    // また、モノクロ状態時のループサウンドを再生/停止する。
    void SetMonochrome(bool enabled)
    {
        // postProcessVolumeが設定されているか確認
        if (postProcessVolume == null)
        {
            Debug.LogWarning("postProcessVolume が設定されていません！");
            return;
        }

        // ColorAdjustmentsコンポーネントを取得し、彩度を設定
        // URPでは、Color Gradingコンポーネント内のSaturationに影響します。
        if (postProcessVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.saturation.value = enabled ? -100f : 0f; // enabledがtrueならモノクロ、falseなら通常
        }

        // 全てのGhostReplayerの一時停止/再開を制御
        foreach (var replayer in FindObjectsOfType<GhostReplayer>())
        {
            if (enabled) replayer.Pause();
            else replayer.Resume();
        }

        // プレイヤーの入力を一時停止/再開
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null) playerController.enabled = !enabled;

        // モノクロループサウンドの再生/停止
        if (loopAudioSource != null && monochromeLoopClip != null)
        {
            if (enabled) // モノクロが有効な場合
            {
                if (!loopAudioSource.isPlaying) // すでに再生中でない場合のみ再生
                {
                    loopAudioSource.clip = monochromeLoopClip;
                    loopAudioSource.loop = true; // ループ再生を確実にする
                    loopAudioSource.Play();
                }
            }
            else // モノクロが無効な場合
            {
                if (loopAudioSource.isPlaying) // 再生中の場合のみ停止
                {
                    loopAudioSource.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning("ループ用オーディオソースまたはモノクロループ用オーディオクリップが設定されていません。");
        }

        // モノクロが解除されたときに、新しい録画の準備を許可する
        if (!enabled)
        {
            isReadyToRecord = true;
        }
    }

    // ゲーム要素をリセットするプライベートメソッド
    private void ResetGameElementsForLoop()
    {
        Debug.Log("[TimeLoopManager] ゲーム要素をタイムループのためにリセットします。");

        // シーン内の全てのPressurePlateを見つけてリセット
        PressurePlate[] allPressurePlates = FindObjectsOfType<PressurePlate>();
        foreach (var plate in allPressurePlates)
        {
            if (plate != null)
            {
                Debug.Log($"[TimeLoopManager] PressurePlate: {plate.name} の ResetPlate() を呼び出します。");
                plate.ResetPlate();
            }
        }

        // シーン内の全てのBrazierSpawnerを見つけてリセット
        BrazierSpawner[] allBrazierSpawners = FindObjectsOfType<BrazierSpawner>();
        foreach (var spawner in allBrazierSpawners)
        {
            if (spawner != null)
            {
                // BrazierSpawner の記録を更新するメソッドを呼び出す
                Debug.Log($"[TimeLoopManager] BrazierSpawner: {spawner.name} の ResetSpawner() を呼び出します。");
                spawner.ResetSpawner(); // これが Brazier の破棄と wasLitByPlayerInPreviousLoop の更新を行う
            }
        }

        // BrazierManagerのリストもクリーンアップ（BrazierSpawnerでBrazierが破棄されるため）
        if (BrazierManager.Instance != null)
        {
            BrazierManager.Instance.braziers.RemoveAll(b => b == null);
            Debug.Log("[BrazierManager] BrazierManagerのリストをクリーンアップしました。");
        }

        // ★修正点★ インベントリ全体のリセットではなく、松明の数量を復元
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RestoreTorchesToInitialQuantity();
        }
    }

    // ゲーム全体の記録をクリアする（例えば、ゲームを最初からやり直す場合など）
    public void ClearAllRecords()
    {
        Debug.Log("[TimeLoopManager] 全ての記録をクリアします。");

        // 全てのBrazierSpawnerの記録をクリア
        BrazierSpawner[] allBrazierSpawners = FindObjectsOfType<BrazierSpawner>();
        foreach (var spawner in allBrazierSpawners)
        {
            if (spawner != null)
            {
                spawner.ClearRecord();
            }
        }

        // 全てのPressurePlateの記録をクリア
        PressurePlate[] allPressurePlates = FindObjectsOfType<PressurePlate>();
        foreach (var plate in allPressurePlates)
        {
            if (plate != null)
            {
                plate.ClearRecord();
            }
        }

        // ★修正点★ インベントリ全体のリセットではなく、松明の数量を復元
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInitialTorchQuantity(); // 初期数量を0として保存
            InventoryManager.Instance.RestoreTorchesToInitialQuantity(); // 0個に復元
        }
    }
}