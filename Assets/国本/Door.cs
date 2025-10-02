using UnityEngine;
using System.Collections; // Coroutine�̂��߂ɕK�v

public class Door : MonoBehaviour
{
    [Tooltip("���̔����J���邽�߂ɉ����Ȃ��Ƃ����Ȃ��{�^��")]
    public DoorButton[] requiredButtons;

    // �h�A�̉�]���ƂȂ�Transform��ݒ肵�Ă��������I��
    [Tooltip("�h�A�̉�]���ƂȂ���GameObject��Transform�������ɐݒ肵�܂��B")]
    public Transform doorPivotTransform;

    [Tooltip("�h�A���J���p�x�i�x�j")]
    public float openAngle = 90f; // ��: 90�x�J��
    [Tooltip("�h�A���J���̂ɂ����鎞�ԁi�b�j")]
    public float openDuration = 1.0f; // ��: 1�b�����ĊJ��
    [Tooltip("�h�A�̊J�����B�h�A�̃��[�J��Y���i������j�����ɉ�]����ꍇ�̕����i1�܂���-1�j")]
    public int rotationDirection = 1; // 1�Ő������i��: Y��+�����j�A-1�ŋt�����i��: Y��-�����j

    private bool isOpened = false;
    private Coroutine currentDoorAnimationCoroutine; // ���ݎ��s���̃h�A�A�j���[�V�����R���[�`��

    // �h�A�s�{�b�g�̏������[�J����]��ۑ� (�J���W�b�N�̍폜�ɔ����A�g�p����Ȃ��Ȃ�܂����A���̃��W�b�N�ŕK�v�Ȃ�c���Ă�������)
    private Quaternion initialPivotLocalRotation;

    void Awake()
    {
        if (doorPivotTransform == null)
        {
            Debug.LogError("Door: doorPivotTransform���ݒ肳��Ă��܂���I�h�A�̃A�j���[�V�������ł��܂���B", this);
            this.enabled = false; // �X�N���v�g�𖳌���
            return; // �G���[�����������ꍇ�͂���ȏ㏈����i�߂Ȃ�
        }

        // doorPivotTransform�̏������[�J����]��ۑ� (���鏈�����Ȃ����߁A���̕ϐ��͒��ڂ͎g���܂��񂪁A�폜�͂��Ă��܂���)
        initialPivotLocalRotation = doorPivotTransform.localRotation;
    }

    private bool AllButtonsPressed()
    {
        foreach (var button in requiredButtons)
        {
            if (button == null || !button.IsPressed)
                return false;
        }
        return true;
    }

    public void OpenDoor()
    {
        if (isOpened) return; // ���ɊJ���Ă����牽�����Ȃ�

        isOpened = true;
        Debug.Log("�����J�����I");

        // ���ɃA�j���[�V���������s���ł���Β�~
        if (currentDoorAnimationCoroutine != null)
        {
            StopCoroutine(currentDoorAnimationCoroutine);
        }
        // �h�A���J���R���[�`�����J�n
        currentDoorAnimationCoroutine = StartCoroutine(AnimateDoor(true));
    }

    

    // �h�A�̃A�j���[�V�����𐧌䂷��R���[�`��
    private IEnumerator AnimateDoor(bool open)
    {
        if (doorPivotTransform == null)
        {
            Debug.LogError("Door: doorPivotTransform���ݒ肳��Ă��܂���I�A�j���[�V�����ł��܂���B", this);
            // �A�j���[�V�������ł��Ȃ��ꍇ�A�����ɔ�\���ɂ���
            gameObject.SetActive(false);
            yield break;
        }

        // doorPivotTransform�̃��[�J����]����ɂ���
        Quaternion startRotation = doorPivotTransform.localRotation;
        Quaternion endRotation;

        // open �� true �̏ꍇ�̂ݏ����i���鏈���͍폜���ꂽ���߁j
        if (open)
        {
            // �J���ŏI�p�x���v�Z (Y���𒆐S�ɉ�])
            // initialPivotLocalRotation����openAngle������]������
            endRotation = initialPivotLocalRotation * Quaternion.Euler(0, openAngle * rotationDirection, 0);
        }
        else
        {
            // ������open��false�̏ꍇ�̏����ł����ACloseDoor()���폜���ꂽ���߁A
            // �_���I�ɂ͂���else�u���b�N�ɂ͓��B���܂���B
            // �������炩�̗��R�œ��B�����ꍇ�A�f�t�H���g�Ńh�A�����i���̈ʒu�ɖ߂��j��������������Ȃ�
            // endRotation = initialPivotLocalRotation;
            // �Ƃ��܂����A����́u���鏈���������v���߁A���B���Ȃ����Ƃ�z�肵�܂��B
            Debug.LogWarning("Door: AnimateDoor�R���[�`����open=false�ŌĂ΂�܂������A���鏈���͍폜����Ă��܂��B");
            yield break; // �O�̂��߃R���[�`�����I��
        }


        float elapsedTime = 0;
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / openDuration);
            doorPivotTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null; // 1�t���[���҂�
        }

        doorPivotTransform.localRotation = endRotation; // �ŏI�ʒu�ɃX�i�b�v

        // �A�j���[�V����������̕\��/��\������
        if (open) // �J���A�j���[�V���������������ꍇ�̂ݔ�\���ɂ���
        {
            //gameObject.SetActive(false); // ���S�ɊJ��������̌����ڂ��\���ɂ���
        }
        // else �u���b�N��open��false�̏ꍇ�̏����ł����ACloseDoor()���폜���ꂽ���ߓ��B���܂���B
    }
}