using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; // For Action event

public class Brazier : MonoBehaviour
{
    // 聖火台の状態が変化したときに発火するイベント
    public event Action<Brazier> OnBrazierStateChanged;

    public float interactDistance = 2f;
    public bool isLit { get; private set; } = false; // 聖火台が点灯しているか
    public GameObject fireEffectPrefab; // 聖火台の炎エフェクトPrefab

    [Tooltip("火のエフェクトを生成する位置のTransformをここに設定します。")]
    public Transform flameSpawnPoint;

    private GameObject currentFlameEffect;

    private float cloneLightCooldown = 0.5f;
    private float lastCloneLightTime = -Mathf.Infinity;

    // BrazierSpawnerから設定される記録値
    private bool _externalWasLitByPlayerInPreviousLoop = false; // 過去のループでプレイヤーによって点灯されたか

    // BrazierSpawnerがこの値を設定するためのプロパティ
    public bool ExternalWasLitByPlayerInPreviousLoop { get => _externalWasLitByPlayerInPreviousLoop; set => _externalWasLitByPlayerInPreviousLoop = value; }


    void Start()
    {
        // 初期状態に基づいて炎の表示を更新
        UpdateFireEffect();

        if (fireEffectPrefab != null)
        {
            Transform spawnParent = (flameSpawnPoint != null) ? flameSpawnPoint : transform;
            Vector3 targetDirection = Vector3.up; // 上方向に向ける例
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // エフェクトを生成し、親を設定
            currentFlameEffect = Instantiate(fireEffectPrefab, spawnParent.position, targetRotation, spawnParent);
            currentFlameEffect.SetActive(false); // 初期状態は非表示
            Debug.Log($"[{name}] 火のエフェクトを生成しました。");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // OnTriggerEnterが呼び出されたことを確認（常に表示）
        Debug.LogWarning($"[{name}]: OnTriggerEnter が呼び出されました。衝突相手: {other.name}, タグ: {other.tag}");

        // クローン（TimeLoopPlayer）が衝突した場合の処理
        if (other.CompareTag("TimeLoopPlayer"))
        {
            // クローンが聖火台に接触したことを示すデバッグログ（常に表示）
            Debug.Log($"[{name}]: クローン ({other.name}) が聖火台に接触しました。");

            // 聖火台の現在の状態を詳細に表示
            Debug.Log($"[{name}]: 聖火台の状態 - activeSelf: {gameObject.activeSelf}, isLit: {isLit}, ExternalWasLitByPlayerInPreviousLoop: {ExternalWasLitByPlayerInPreviousLoop}");

            // 聖火台がアクティブ（表示されている） かつ まだ火が灯されていない場合のみ
            if (gameObject.activeSelf && !isLit)
            {
                // クールダウンの状態を確認
                float timeSinceLastLight = Time.time - lastCloneLightTime;
                Debug.Log($"[{name}]: クローン点灯試行 - timeSinceLastLight: {timeSinceLastLight:F2}, cloneLightCooldown: {cloneLightCooldown}");

                // クールダウン期間中でなければ火を灯す試行
                if (timeSinceLastLight >= cloneLightCooldown)
                {
                    // ★修正点★ ExternalWasLitByPlayerInPreviousLoop が true なら点灯
                    if (ExternalWasLitByPlayerInPreviousLoop) // 過去にプレイヤーが点灯した記録があるか
                    {
                        LightBrazier();
                        lastCloneLightTime = Time.time; // 最後に火を灯した時間を記録
                        Debug.Log($"{name}: クローンがぶつかって火が灯されました！");
                    }
                    else
                    {
                        Debug.Log($"[{name}]: クローンがぶつかりましたが、過去に火をつけた記録がないため反応しません。");
                    }
                }
                else
                {
                    Debug.Log($"[{name}]: クローンが接触しましたが、クールダウン中 ({timeSinceLastLight:F2} / {cloneLightCooldown:F2}秒) のため火はつきません。");
                }
            }
            else // if (gameObject.activeSelf && !isLit) の条件が満たされなかった場合
            {
                // 聖火台が非アクティブ、または既に点灯済みの場合のログ
                if (!gameObject.activeSelf)
                {
                    Debug.Log($"[{name}]: 聖火台 ({other.name}) は非アクティブなため、クローンは火を灯せません。");
                }
                else if (isLit)
                {
                    Debug.Log($"[{name}]: 聖火台 ({other.name}) は既に点灯済みのため、クローンは火を灯せません。");
                }
            }
        }
    }

    void Update()
    {
        // プレイヤーがFキー (またはJoystickButton2) で聖火台に火を灯そうとする
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            // 聖火台がアクティブ（表示されている） かつ まだ火が灯されていない場合のみインタラクト可能
            if (gameObject.activeSelf && !isLit && Vector3.Distance(Camera.main.transform.position, transform.position) <= interactDistance)
            {
                TryLightBrazier(); // プレイヤーのアクション
            }
        }
    }

    private void TryLightBrazier()
    {
        // インベントリに松明があるかチェック
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem("Torch"))
        {
            LightBrazier();

            // ★修正点★ BrazierSpawner にプレイヤーが点灯したことを通知
            BrazierSpawner parentSpawner = GetComponentInParent<BrazierSpawner>();
            if (parentSpawner != null)
            {
                parentSpawner.NotifyPlayerLitBrazier(); // 新しいメソッドを呼び出す
                Debug.Log($"[{name}]: TryLightBrazier - 親のBrazierSpawnerにプレイヤーが点灯したことを通知しました。");
            }
            else
            {
                Debug.LogWarning($"[{name}]: TryLightBrazier - 親のBrazierSpawnerが見つかりません。プレイヤー点灯記録を更新できません。");
            }

            InventoryManager.Instance.RemoveItem("Torch"); // 松明を1つ消費
            Debug.Log($"{name}: プレイヤーがアクションで火を灯しました！松明を消費しました。");
        }
        else
        {
            Debug.Log("火を灯すには、インベントリに松明が必要です。");
        }
    }

    public void LightBrazier()
    {
        if (!isLit)
        {
            isLit = true;
            Debug.Log($"{name} に火が点きました！");
            UpdateFireEffect();
            OnBrazierStateChanged?.Invoke(this); // イベントを発火
        }
    }

    public void ExtinguishBrazier()
    {
        if (isLit)
        {
            isLit = false;
            Debug.Log($"{name} の火が消えました。");
            UpdateFireEffect();
            OnBrazierStateChanged?.Invoke(this); // イベントを発火
        }
    }

    private void UpdateFireEffect()
    {
        if (currentFlameEffect != null)
        {
            currentFlameEffect.SetActive(isLit);
        }
    }

    // 聖火台の表示/非表示を制御 (BrazierSpawnerが使用)
    public void SetActiveState(bool active)
    {
        gameObject.SetActive(active);
    }
}

