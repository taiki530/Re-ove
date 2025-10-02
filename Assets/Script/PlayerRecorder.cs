using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    [System.Serializable]
    public class FrameData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 moveInput;
        public bool jumpPressed;
        public float timeStamp; // Time.time で記録
    }

    public List<FrameData> recordedFrames = new List<FrameData>();
    private float checkpointStartTime = -1f; // チェックポイント通過時間
    public float maxRecordDuration = 30f;

    void FixedUpdate() // 安定再現のため FixedUpdate 使用
    {
        if (checkpointStartTime < 0) return;

        float now = Time.time;

        var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool jumpPressed = Input.GetKey(KeyCode.Space);

        recordedFrames.Add(new FrameData
        {
            position = transform.root.position,     
            rotation = transform.root.rotation,     
            moveInput = moveInput,
            jumpPressed = jumpPressed,
            timeStamp = now
        });


        // maxRecordDuration を超えた古いデータを削除
        recordedFrames.RemoveAll(f => f.timeStamp < now - maxRecordDuration);
    }

    /// <summary>
    /// 録画をチェックポイント通過から開始
    /// </summary>
    public void StartRecordingFromCheckpoint()
    {
        checkpointStartTime = Time.time;

        // 録画フレームもリセット（厳密にチェックポイントから記録）
        recordedFrames.Clear();
    }

    /// <summary>
    /// チェックポイント以降の録画データのみ返す
    /// </summary>
    public List<FrameData> GetFramesSinceCheckpoint()
    {
        return recordedFrames.FindAll(f => f.timeStamp >= checkpointStartTime);
    }

    /// <summary>
    /// 録画が存在するか
    /// </summary>
    public bool HasRecording()
    {
        return checkpointStartTime >= 0 && recordedFrames.Count > 0;
    }
}
