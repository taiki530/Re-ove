using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSightChecker : MonoBehaviour
{
    [SerializeField] private float _sightAngle = 90.0f;
    [SerializeField] private float _maxDistance = 10.0f;

    [SerializeField] private float damage = 100f;

    private GameObject target;
    private PlayerStatus player;

    // TimeLoopManager�̎Q�� (�t���b�V���G�t�F�N�g���Ăяo�����߂ɕK�v)
    private TimeLoopManager timeLoopManager;

    [Header("Flash Audio Settings")]
    [SerializeField] private AudioSource sightFlashAudioSource; // ���̃I�u�W�F�N�g��AudioSource
    [SerializeField] private AudioClip sightFlashSoundClip;   // �Đ��������T�E���h�N���b�v

    // === ���ǉ��F�t���b�V���̑҂����ԁiFlashEffect��duration�ƍ��킹��j�� ===
    // FlashEffect��flashDuration�̔������炢���ǂ��ł��傤�B
    // ��: FlashEffect��flashDuration��0.2f�Ȃ�A0.1f��ݒ�
    [SerializeField] private float waitBeforeReloadDuration = 0.1f;
    // ==============================================================

    private bool hasGivenDamage = false;

    private void Start()
    {
        target = GameObject.Find("PlayerRoot");
        player = FindObjectOfType<PlayerStatus>();
        timeLoopManager = TimeLoopManager.Instance;

        if (timeLoopManager == null)
        {
            Debug.LogError("TimeLoopManager.Instance ��������܂���I�V�[����TimeLoopManager������܂���B");
        }

        if (sightFlashAudioSource == null)
        {
            sightFlashAudioSource = GetComponent<AudioSource>();
            if (sightFlashAudioSource == null)
            {
                sightFlashAudioSource = gameObject.AddComponent<AudioSource>();
            }
            sightFlashAudioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (IsVisible())
        {
            if (!hasGivenDamage)
            {
                if (player != null)
                {
                    player.TakeDamage(damage, DeathCause.Found);

                    // ���R���[�`�����Ăяo���āA�t���b�V���\���ƃV�[�������[�h�𓯊������遚
                    StartCoroutine(HandleDiscoveryAndReload());
                }
                Debug.Log("GAME OVER");
                hasGivenDamage = true;
            }
        }
        else
        {
            hasGivenDamage = false;
        }
    }

    // === ���V�����ǉ�����R���[�`���� ===
    private IEnumerator HandleDiscoveryAndReload()
    {
        // �t���b�V���G�t�F�N�g�̌Ăяo��
        if (timeLoopManager != null && timeLoopManager.flashEffect != null)
        {
            // FlashEffect��DoFlash�R���[�`�����J�n
            // FlashEffect.cs���C�����Ȃ����߁A�����ł�DoFlash�̊�����҂��Ȃ��B
            // DoFlash�������łǂ̂悤�ɏI�����邩��PlayerSightChecker����͊֗^���Ȃ��B
            StartCoroutine(timeLoopManager.flashEffect.DoFlash());
        }
        else
        {
            Debug.LogWarning("FlashEffect��TimeLoopManager�ɐݒ肳��Ă��Ȃ����ATimeLoopManager��������܂���B");
        }

        // �T�E���h���Đ�
        if (sightFlashAudioSource != null && sightFlashSoundClip != null)
        {
            sightFlashAudioSource.PlayOneShot(sightFlashSoundClip);
        }
        else
        {
            if (sightFlashAudioSource == null) Debug.LogWarning("PlayerSightChecker: sightFlashAudioSource ���ݒ肳��Ă��܂���B");
            if (sightFlashSoundClip == null) Debug.LogWarning("PlayerSightChecker: sightFlashSoundClip ���ݒ肳��Ă��܂���.");
        }

        Debug.Log("�v���C���[����������܂����I�t���b�V�����\�������܂őҋ@���܂��B");

        // �������Ńt���b�V������ʂ𕢂��܂ł̎��ԑҋ@���遚
        // waitBeforeReloadDuration �̒l�́AFlashEffect�̃t�F�[�h�C�����Ԃɍ��킹�Ē������Ă��������B
        yield return new WaitForSeconds(waitBeforeReloadDuration);

        // �ҋ@��A�X�e�[�W���X�^�[�g�̃��W�b�N�����s
        Debug.Log("�t���b�V����A�X�e�[�W�����X�^�[�g���܂��B");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    // ===================================

    public bool IsVisible()
    {
        if (this.gameObject.CompareTag("TimeLoopPlayer") && target != null)
        {
            Vector3 selfPos = transform.position;
            Vector3 targetPos = target.transform.position;

            Vector3 selfDir = transform.forward;
            Vector3 targetDir = targetPos - selfPos;
            float targetDistance = targetDir.magnitude;

            float cosHalf = Mathf.Cos(_sightAngle * 0.5f * Mathf.Deg2Rad);
            float innerProduct = Vector3.Dot(selfDir, targetDir.normalized);

            if (innerProduct > cosHalf && targetDistance <= _maxDistance)
            {
                if (Physics.Raycast(selfPos, targetDir.normalized, out RaycastHit hit, _maxDistance))
                {
                    if (hit.collider.gameObject == target)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}