using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GhostReplayer : MonoBehaviour
{
    private List<PlayerRecorder.FrameData> playbackData;
    private int frameIndex = 0;

    private CharacterController controller;
    private Vector3 velocity; // このvelocityはCharacterController内部の移動に使われているようですが、アニメーション用とは別にします

    private float startTime;
    private bool isPlaying = false;

    private float colliderDisableTime = 0.5f;
    private float colliderTimer = 0f;

    private float DelayTime = 0.0f; //ゴースト遅延時間（西尾編集）
    private int PauseTime = 0; //ゴースト一時停止時間（西尾編集）
    private bool GrenadePaused = false; //ゴースト一時停止中（西尾編集）

    // AnimatorControllerに渡すためのプロパティを追加
    public float CurrentHorizontalSpeed { get; private set; } = 0f;
    public bool IsGrounded { get; private set; } = false; // CharacterControllerのisGroundedを使うのが一般的

    /// <summary>
    /// ゴーストの録画再生を開始します。
    /// </summary>
    /// <param name="data">再生するフレームデータ</param>
    /// <param name="sharedStartTime">すべてのゴーストが同期して再生を開始するための基準時間</param>
    public void PlayRecording(List<PlayerRecorder.FrameData> data, float sharedStartTime)
    {
        if (data == null || data.Count < 2)
        {
            Debug.LogWarning("再生データが不正です");
            return;
        }

        playbackData = data;
        frameIndex = 0;
        isPlaying = false; // PlayRecording時点ではまだ再生開始しない (Resume()でisPlaying=trueにする)
        colliderTimer = 0f;

        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("Ghost に CharacterController が必要です。");
            return;
        }

        controller.enabled = false; // 初期位置設定のために一時的に無効化
        transform.position = playbackData[0].position;
        transform.rotation = playbackData[0].rotation;
        controller.enabled = true; // 有効化に戻す

        velocity = Vector3.zero; // CharacterControllerのvelocityを初期化

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        startTime = Time.time - playbackData[0].timeStamp;

        // 追加: 初期速度と接地状態を設定
        CurrentHorizontalSpeed = 0f;
        IsGrounded = controller.isGrounded; // 初期状態の接地判定
    }

    /// <summary>
    /// ゴーストの再生を再開します。
    /// </summary>
    public void Resume()
    {
        Debug.Log("GhostReplayer: Resume() called");

        // playbackDataがnullでなければ再生を開始
        if (playbackData != null && !isPlaying)
        {
            StartPlayback();
        }
        this.enabled = true; // MonoBehaviour全体を有効にする (Updateが動き出す)
    }

    /// <summary>
    /// ゴーストの再生を一時停止します。
    /// </summary>
    public void Pause()
    {
        Debug.Log("GhostReplayer: Pause() called");
        this.enabled = false; // MonoBehaviour全体を無効にする (Updateが止まる)
        CurrentHorizontalSpeed = 0f; // ポーズ中は速度を0にする
    }

    public void GrenadePause(float time) //グレネード一時停止（西尾編集）
    {
        GrenadePaused = true;
        DelayTime += time;
        PauseTime += (int)time * 60;
        CurrentHorizontalSpeed = 0f; // グレネード一時停止中も速度を0にする
    }

    /// <summary>
    /// 実際の再生ロジックを開始します。
    /// </summary>
    private void StartPlayback()
    {
        // playbackDataがnullでないことを保証するチェックを追加
        if (playbackData == null || playbackData.Count < 2)
        {
            Debug.LogWarning("GhostReplayer: 再生データが見つかりません。Playbackを中止します。");
            this.enabled = false; // スクリプトを無効化してエラーを避ける
            return;
        }

        startTime = Time.time - playbackData[0].timeStamp;
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying || playbackData == null || playbackData.Count < 2)
        {
            // isPlayingがfalseになったら、このスクリプトを無効化して負荷を軽減
            if (this.enabled) this.enabled = false;
            CurrentHorizontalSpeed = 0f; // 再生終了時も速度を0に
            return;
        }

        // 現在の位置を保存 (速度計算用)
        Vector3 previousPosition = transform.position;

        if (GrenadePaused == false) //グレネード一時停止でなければ動く
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer >= colliderDisableTime)
            {
                Collider col = GetComponent<Collider>();
                if (col != null && !col.enabled)
                    col.enabled = true;
            }

            float elapsed = Time.time - startTime - DelayTime; //一時停止した分遅延させる

            while (frameIndex < playbackData.Count - 1 && playbackData[frameIndex + 1].timeStamp <= elapsed)
            {
                frameIndex++;
            }

            if (frameIndex >= playbackData.Count - 1)
            {
                isPlaying = false;
                if (this.enabled) this.enabled = false; // 再生終了したら自身を無効化
                CurrentHorizontalSpeed = 0f; // 再生終了時も速度を0に
                return;
            }

            var from = playbackData[frameIndex];
            var to = playbackData[frameIndex + 1];

            float frameTimeDelta = to.timeStamp - from.timeStamp;
            float t = (elapsed - from.timeStamp) / frameTimeDelta;

            Vector3 interpPos = Vector3.Lerp(from.position, to.position, t);
            Quaternion interpRot = Quaternion.Slerp(from.rotation, to.rotation, t);

            // CharacterControllerを一旦無効化して位置と回転を設定
            // controller.Move()を使わない場合は、これを行うのが一般的
            controller.enabled = false;
            transform.position = interpPos;
            transform.rotation = interpRot;
            controller.enabled = true;

            // ★速度と接地状態の計算と公開★
            Vector3 currentPosition = transform.position;
            Vector3 displacement = currentPosition - previousPosition;

            // 地面と水平方向の速度のみを計算 (Y成分を無視)
            Vector3 horizontalVelocity = new Vector3(displacement.x, 0f, displacement.z) / Time.deltaTime;
            CurrentHorizontalSpeed = horizontalVelocity.magnitude;

            // CharacterControllerのisGroundedプロパティから接地状態を取得
            IsGrounded = controller.isGrounded;
        }
        else // グレネード一時停止中の処理
        {
            CurrentHorizontalSpeed = 0f; // 一時停止中は速度を0に
            PauseTime -= 1;
            if (PauseTime <= 0)
            {
                PauseTime = 0;
                GrenadePaused = false;
            }
        }
    }

    /// <summary>
    /// 現在のポーズ状態を取得します。（MonoBehaviour.enabledを使うのが推奨）
    /// </summary>
    public bool GetPause()
    {
        return !this.enabled; // MonoBehaviourが有効ならポーズ中でない
    }

    public bool GetGrenadePause()
    {
        return GrenadePaused; // グレネード一時停止ゲッター（西尾編集）
    }

    public bool IsReplaying => isPlaying;
}