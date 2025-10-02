using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GhostReplayer : MonoBehaviour
{
    private List<PlayerRecorder.FrameData> playbackData;
    private int frameIndex = 0;

    private CharacterController controller;
    private Vector3 velocity; // ����velocity��CharacterController�����̈ړ��Ɏg���Ă���悤�ł����A�A�j���[�V�����p�Ƃ͕ʂɂ��܂�

    private float startTime;
    private bool isPlaying = false;

    private float colliderDisableTime = 0.5f;
    private float colliderTimer = 0f;

    private float DelayTime = 0.0f; //�S�[�X�g�x�����ԁi�����ҏW�j
    private int PauseTime = 0; //�S�[�X�g�ꎞ��~���ԁi�����ҏW�j
    private bool GrenadePaused = false; //�S�[�X�g�ꎞ��~���i�����ҏW�j

    // AnimatorController�ɓn�����߂̃v���p�e�B��ǉ�
    public float CurrentHorizontalSpeed { get; private set; } = 0f;
    public bool IsGrounded { get; private set; } = false; // CharacterController��isGrounded���g���̂���ʓI

    /// <summary>
    /// �S�[�X�g�̘^��Đ����J�n���܂��B
    /// </summary>
    /// <param name="data">�Đ�����t���[���f�[�^</param>
    /// <param name="sharedStartTime">���ׂẴS�[�X�g���������čĐ����J�n���邽�߂̊����</param>
    public void PlayRecording(List<PlayerRecorder.FrameData> data, float sharedStartTime)
    {
        if (data == null || data.Count < 2)
        {
            Debug.LogWarning("�Đ��f�[�^���s���ł�");
            return;
        }

        playbackData = data;
        frameIndex = 0;
        isPlaying = false; // PlayRecording���_�ł͂܂��Đ��J�n���Ȃ� (Resume()��isPlaying=true�ɂ���)
        colliderTimer = 0f;

        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("Ghost �� CharacterController ���K�v�ł��B");
            return;
        }

        controller.enabled = false; // �����ʒu�ݒ�̂��߂Ɉꎞ�I�ɖ�����
        transform.position = playbackData[0].position;
        transform.rotation = playbackData[0].rotation;
        controller.enabled = true; // �L�����ɖ߂�

        velocity = Vector3.zero; // CharacterController��velocity��������

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        startTime = Time.time - playbackData[0].timeStamp;

        // �ǉ�: �������x�Ɛڒn��Ԃ�ݒ�
        CurrentHorizontalSpeed = 0f;
        IsGrounded = controller.isGrounded; // ������Ԃ̐ڒn����
    }

    /// <summary>
    /// �S�[�X�g�̍Đ����ĊJ���܂��B
    /// </summary>
    public void Resume()
    {
        Debug.Log("GhostReplayer: Resume() called");

        // playbackData��null�łȂ���΍Đ����J�n
        if (playbackData != null && !isPlaying)
        {
            StartPlayback();
        }
        this.enabled = true; // MonoBehaviour�S�̂�L���ɂ��� (Update�������o��)
    }

    /// <summary>
    /// �S�[�X�g�̍Đ����ꎞ��~���܂��B
    /// </summary>
    public void Pause()
    {
        Debug.Log("GhostReplayer: Pause() called");
        this.enabled = false; // MonoBehaviour�S�̂𖳌��ɂ��� (Update���~�܂�)
        CurrentHorizontalSpeed = 0f; // �|�[�Y���͑��x��0�ɂ���
    }

    public void GrenadePause(float time) //�O���l�[�h�ꎞ��~�i�����ҏW�j
    {
        GrenadePaused = true;
        DelayTime += time;
        PauseTime += (int)time * 60;
        CurrentHorizontalSpeed = 0f; // �O���l�[�h�ꎞ��~�������x��0�ɂ���
    }

    /// <summary>
    /// ���ۂ̍Đ����W�b�N���J�n���܂��B
    /// </summary>
    private void StartPlayback()
    {
        // playbackData��null�łȂ����Ƃ�ۏ؂���`�F�b�N��ǉ�
        if (playbackData == null || playbackData.Count < 2)
        {
            Debug.LogWarning("GhostReplayer: �Đ��f�[�^��������܂���BPlayback�𒆎~���܂��B");
            this.enabled = false; // �X�N���v�g�𖳌������ăG���[�������
            return;
        }

        startTime = Time.time - playbackData[0].timeStamp;
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying || playbackData == null || playbackData.Count < 2)
        {
            // isPlaying��false�ɂȂ�����A���̃X�N���v�g�𖳌������ĕ��ׂ��y��
            if (this.enabled) this.enabled = false;
            CurrentHorizontalSpeed = 0f; // �Đ��I���������x��0��
            return;
        }

        // ���݂̈ʒu��ۑ� (���x�v�Z�p)
        Vector3 previousPosition = transform.position;

        if (GrenadePaused == false) //�O���l�[�h�ꎞ��~�łȂ���Γ���
        {
            colliderTimer += Time.deltaTime;
            if (colliderTimer >= colliderDisableTime)
            {
                Collider col = GetComponent<Collider>();
                if (col != null && !col.enabled)
                    col.enabled = true;
            }

            float elapsed = Time.time - startTime - DelayTime; //�ꎞ��~�������x��������

            while (frameIndex < playbackData.Count - 1 && playbackData[frameIndex + 1].timeStamp <= elapsed)
            {
                frameIndex++;
            }

            if (frameIndex >= playbackData.Count - 1)
            {
                isPlaying = false;
                if (this.enabled) this.enabled = false; // �Đ��I�������玩�g�𖳌���
                CurrentHorizontalSpeed = 0f; // �Đ��I���������x��0��
                return;
            }

            var from = playbackData[frameIndex];
            var to = playbackData[frameIndex + 1];

            float frameTimeDelta = to.timeStamp - from.timeStamp;
            float t = (elapsed - from.timeStamp) / frameTimeDelta;

            Vector3 interpPos = Vector3.Lerp(from.position, to.position, t);
            Quaternion interpRot = Quaternion.Slerp(from.rotation, to.rotation, t);

            // CharacterController����U���������Ĉʒu�Ɖ�]��ݒ�
            // controller.Move()���g��Ȃ��ꍇ�́A������s���̂���ʓI
            controller.enabled = false;
            transform.position = interpPos;
            transform.rotation = interpRot;
            controller.enabled = true;

            // �����x�Ɛڒn��Ԃ̌v�Z�ƌ��J��
            Vector3 currentPosition = transform.position;
            Vector3 displacement = currentPosition - previousPosition;

            // �n�ʂƐ��������̑��x�݂̂��v�Z (Y�����𖳎�)
            Vector3 horizontalVelocity = new Vector3(displacement.x, 0f, displacement.z) / Time.deltaTime;
            CurrentHorizontalSpeed = horizontalVelocity.magnitude;

            // CharacterController��isGrounded�v���p�e�B����ڒn��Ԃ��擾
            IsGrounded = controller.isGrounded;
        }
        else // �O���l�[�h�ꎞ��~���̏���
        {
            CurrentHorizontalSpeed = 0f; // �ꎞ��~���͑��x��0��
            PauseTime -= 1;
            if (PauseTime <= 0)
            {
                PauseTime = 0;
                GrenadePaused = false;
            }
        }
    }

    /// <summary>
    /// ���݂̃|�[�Y��Ԃ��擾���܂��B�iMonoBehaviour.enabled���g���̂������j
    /// </summary>
    public bool GetPause()
    {
        return !this.enabled; // MonoBehaviour���L���Ȃ�|�[�Y���łȂ�
    }

    public bool GetGrenadePause()
    {
        return GrenadePaused; // �O���l�[�h�ꎞ��~�Q�b�^�[�i�����ҏW�j
    }

    public bool IsReplaying => isPlaying;
}