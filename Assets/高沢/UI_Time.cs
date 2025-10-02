using System;
using UnityEngine;
using UnityEngine.UI;

public class DeltaTimeClock : MonoBehaviour
{
    [SerializeField]
    private Transform hourHand;

    [SerializeField]
    private Transform minuteHand;

    [SerializeField]
    private Transform secondHand;

    // SystemTimer �X�N���v�g�ւ̎Q��
    public SystemTimer systemTimer; // �C���X�y�N�^�[����h���b�O���h���b�v�ŃA�T�C��

    private float elapsedTime; // UI_Time���g�̌o�ߎ��Ԃ͂����s�v�ɂȂ�

    private void Start()
    {
        // SystemTimer ���A�T�C������Ă��邱�Ƃ��m�F
        //NULL�`�F�b�N
       
        if (systemTimer == null)
        {
            Debug.LogError("SystemTimer ���A�T�C������Ă��܂���I");
            // Fallback: �����A�T�C������Ă��Ȃ���΁A���g�̌o�ߎ��Ԃŏ�����
            DateTime now = DateTime.Now;
            elapsedTime = now.Hour * 3600 + now.Minute * 60 + now.Second + now.Millisecond / 1000f;
        }
        else
        {
            //������

            // SystemTimer���A�T�C������Ă���ꍇ�ł��AStart���_�ł̏����l��n��
            // SystemTimer��elapsedTime���܂�����������Ă��Ȃ��\�����l�����A
            // �����ł�DateTime.Now���x�[�X�ɂ����l���g���̂����S�ł��B
            DateTime now = DateTime.Now;
            elapsedTime = now.Hour * 3600 + now.Minute * 60 + now.Second + now.Millisecond / 1000f;
        }

        // UpdateClockHands() �ɏ����l��n���ČĂяo��
        // �����ł�elapsedTime�𒼐ړn���܂��B

        UpdateClockHands(elapsedTime);
    }

    private void Update()
    {
        // SystemTimer �� elapsedtime ���g�p
        if (systemTimer != null)
        {
            elapsedTime = systemTimer.ElapsedTime; // SystemTimer �̌o�ߎ��Ԃ��擾
        }
        else
        {
            // Fallback: SystemTimer ���Ȃ���Ύ��g��elapsedTime�����Z
            elapsedTime += Time.deltaTime;
        }


        // 24���Ԃ𒴂����烊�Z�b�g�i�ȗ��\�ASystemTimer��elapsedtime�̓��Z�b�g����Ȃ��̂Œ��Ӂj
        // UI_Time���ŏ��SystemTimer�̌o�ߎ��Ԃ��g���ꍇ�A���̃��Z�b�g�͈Ӗ����Ȃ��Ȃ�܂��B
        // �������v�Ƃ���24���Ԏ����ŕ\���������Ȃ�AelapsedTime�����W�������Z�Œ������܂��B
        float displayTime = elapsedTime;
        if (displayTime >= 86400f) // 24 * 60 * 60
        {
            displayTime = displayTime % 86400f; // 24���ԂŃ��[�v������
        }


        UpdateClockHands(displayTime);
    }

    private void UpdateClockHands(float totalSeconds) // �����Ōo�ߎ��Ԃ��󂯎��悤�ɕύX
    {
        // ���E���E�b���Čv�Z
        int hours = (int)(totalSeconds / 3600f);
        int minutes = (int)((totalSeconds % 3600f) / 60f);
        float seconds = totalSeconds % 60f;
    

        // �e�j�̊p�x��ݒ�iZ����]�j
        if (secondHand != null)
        {
            float secAngle = 360f - (seconds / 60f) * 360f;
            secondHand.localEulerAngles = new Vector3(0, 0, secAngle);
        }

        if (minuteHand != null)
        {
            float minAngle = 360f - ((minutes + seconds / 60f) / 60f) * 360f;
            minuteHand.localEulerAngles = new Vector3(0, 0, minAngle);
        }

        if (hourHand != null)
        {
            float hourAngle = 360f - ((hours % 12 + minutes / 60f) / 12f) * 360f;
            hourHand.localEulerAngles = new Vector3(0, 0, hourAngle);

        }
    }
}