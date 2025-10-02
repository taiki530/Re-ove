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
        public float timeStamp; // Time.time �ŋL�^
    }

    public List<FrameData> recordedFrames = new List<FrameData>();
    private float checkpointStartTime = -1f; // �`�F�b�N�|�C���g�ʉߎ���
    public float maxRecordDuration = 30f;

    void FixedUpdate() // ����Č��̂��� FixedUpdate �g�p
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


        // maxRecordDuration �𒴂����Â��f�[�^���폜
        recordedFrames.RemoveAll(f => f.timeStamp < now - maxRecordDuration);
    }

    /// <summary>
    /// �^����`�F�b�N�|�C���g�ʉ߂���J�n
    /// </summary>
    public void StartRecordingFromCheckpoint()
    {
        checkpointStartTime = Time.time;

        // �^��t���[�������Z�b�g�i�����Ƀ`�F�b�N�|�C���g����L�^�j
        recordedFrames.Clear();
    }

    /// <summary>
    /// �`�F�b�N�|�C���g�ȍ~�̘^��f�[�^�̂ݕԂ�
    /// </summary>
    public List<FrameData> GetFramesSinceCheckpoint()
    {
        return recordedFrames.FindAll(f => f.timeStamp >= checkpointStartTime);
    }

    /// <summary>
    /// �^�悪���݂��邩
    /// </summary>
    public bool HasRecording()
    {
        return checkpointStartTime >= 0 && recordedFrames.Count > 0;
    }
}
