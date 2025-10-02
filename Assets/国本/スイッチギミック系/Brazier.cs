using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; // For Action event

public class Brazier : MonoBehaviour
{
    // ���Α�̏�Ԃ��ω������Ƃ��ɔ��΂���C�x���g
    public event Action<Brazier> OnBrazierStateChanged;

    public float interactDistance = 2f;
    public bool isLit { get; private set; } = false; // ���Α䂪�_�����Ă��邩
    public GameObject fireEffectPrefab; // ���Α�̉��G�t�F�N�gPrefab

    [Tooltip("�΂̃G�t�F�N�g�𐶐�����ʒu��Transform�������ɐݒ肵�܂��B")]
    public Transform flameSpawnPoint;

    private GameObject currentFlameEffect;

    private float cloneLightCooldown = 0.5f;
    private float lastCloneLightTime = -Mathf.Infinity;

    // BrazierSpawner����ݒ肳���L�^�l
    private bool _externalWasLitByPlayerInPreviousLoop = false; // �ߋ��̃��[�v�Ńv���C���[�ɂ���ē_�����ꂽ��

    // BrazierSpawner�����̒l��ݒ肷�邽�߂̃v���p�e�B
    public bool ExternalWasLitByPlayerInPreviousLoop { get => _externalWasLitByPlayerInPreviousLoop; set => _externalWasLitByPlayerInPreviousLoop = value; }


    void Start()
    {
        // ������ԂɊ�Â��ĉ��̕\�����X�V
        UpdateFireEffect();

        if (fireEffectPrefab != null)
        {
            Transform spawnParent = (flameSpawnPoint != null) ? flameSpawnPoint : transform;
            Vector3 targetDirection = Vector3.up; // ������Ɍ������
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // �G�t�F�N�g�𐶐����A�e��ݒ�
            currentFlameEffect = Instantiate(fireEffectPrefab, spawnParent.position, targetRotation, spawnParent);
            currentFlameEffect.SetActive(false); // ������Ԃ͔�\��
            Debug.Log($"[{name}] �΂̃G�t�F�N�g�𐶐����܂����B");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // OnTriggerEnter���Ăяo���ꂽ���Ƃ��m�F�i��ɕ\���j
        Debug.LogWarning($"[{name}]: OnTriggerEnter ���Ăяo����܂����B�Փˑ���: {other.name}, �^�O: {other.tag}");

        // �N���[���iTimeLoopPlayer�j���Փ˂����ꍇ�̏���
        if (other.CompareTag("TimeLoopPlayer"))
        {
            // �N���[�������Α�ɐڐG�������Ƃ������f�o�b�O���O�i��ɕ\���j
            Debug.Log($"[{name}]: �N���[�� ({other.name}) �����Α�ɐڐG���܂����B");

            // ���Α�̌��݂̏�Ԃ��ڍׂɕ\��
            Debug.Log($"[{name}]: ���Α�̏�� - activeSelf: {gameObject.activeSelf}, isLit: {isLit}, ExternalWasLitByPlayerInPreviousLoop: {ExternalWasLitByPlayerInPreviousLoop}");

            // ���Α䂪�A�N�e�B�u�i�\������Ă���j ���� �܂��΂�������Ă��Ȃ��ꍇ�̂�
            if (gameObject.activeSelf && !isLit)
            {
                // �N�[���_�E���̏�Ԃ��m�F
                float timeSinceLastLight = Time.time - lastCloneLightTime;
                Debug.Log($"[{name}]: �N���[���_�����s - timeSinceLastLight: {timeSinceLastLight:F2}, cloneLightCooldown: {cloneLightCooldown}");

                // �N�[���_�E�����Ԓ��łȂ���Ή΂𓔂����s
                if (timeSinceLastLight >= cloneLightCooldown)
                {
                    // ���C���_�� ExternalWasLitByPlayerInPreviousLoop �� true �Ȃ�_��
                    if (ExternalWasLitByPlayerInPreviousLoop) // �ߋ��Ƀv���C���[���_�������L�^�����邩
                    {
                        LightBrazier();
                        lastCloneLightTime = Time.time; // �Ō�ɉ΂𓔂������Ԃ��L�^
                        Debug.Log($"{name}: �N���[�����Ԃ����ĉ΂�������܂����I");
                    }
                    else
                    {
                        Debug.Log($"[{name}]: �N���[�����Ԃ���܂������A�ߋ��ɉ΂������L�^���Ȃ����ߔ������܂���B");
                    }
                }
                else
                {
                    Debug.Log($"[{name}]: �N���[�����ڐG���܂������A�N�[���_�E���� ({timeSinceLastLight:F2} / {cloneLightCooldown:F2}�b) �̂��߉΂͂��܂���B");
                }
            }
            else // if (gameObject.activeSelf && !isLit) �̏�������������Ȃ������ꍇ
            {
                // ���Α䂪��A�N�e�B�u�A�܂��͊��ɓ_���ς݂̏ꍇ�̃��O
                if (!gameObject.activeSelf)
                {
                    Debug.Log($"[{name}]: ���Α� ({other.name}) �͔�A�N�e�B�u�Ȃ��߁A�N���[���͉΂𓔂��܂���B");
                }
                else if (isLit)
                {
                    Debug.Log($"[{name}]: ���Α� ({other.name}) �͊��ɓ_���ς݂̂��߁A�N���[���͉΂𓔂��܂���B");
                }
            }
        }
    }

    void Update()
    {
        // �v���C���[��F�L�[ (�܂���JoystickButton2) �Ő��Α�ɉ΂𓔂����Ƃ���
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            // ���Α䂪�A�N�e�B�u�i�\������Ă���j ���� �܂��΂�������Ă��Ȃ��ꍇ�̂݃C���^���N�g�\
            if (gameObject.activeSelf && !isLit && Vector3.Distance(Camera.main.transform.position, transform.position) <= interactDistance)
            {
                TryLightBrazier(); // �v���C���[�̃A�N�V����
            }
        }
    }

    private void TryLightBrazier()
    {
        // �C���x���g���ɏ��������邩�`�F�b�N
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem("Torch"))
        {
            LightBrazier();

            // ���C���_�� BrazierSpawner �Ƀv���C���[���_���������Ƃ�ʒm
            BrazierSpawner parentSpawner = GetComponentInParent<BrazierSpawner>();
            if (parentSpawner != null)
            {
                parentSpawner.NotifyPlayerLitBrazier(); // �V�������\�b�h���Ăяo��
                Debug.Log($"[{name}]: TryLightBrazier - �e��BrazierSpawner�Ƀv���C���[���_���������Ƃ�ʒm���܂����B");
            }
            else
            {
                Debug.LogWarning($"[{name}]: TryLightBrazier - �e��BrazierSpawner��������܂���B�v���C���[�_���L�^���X�V�ł��܂���B");
            }

            InventoryManager.Instance.RemoveItem("Torch"); // ������1����
            Debug.Log($"{name}: �v���C���[���A�N�V�����ŉ΂𓔂��܂����I����������܂����B");
        }
        else
        {
            Debug.Log("�΂𓔂��ɂ́A�C���x���g���ɏ������K�v�ł��B");
        }
    }

    public void LightBrazier()
    {
        if (!isLit)
        {
            isLit = true;
            Debug.Log($"{name} �ɉ΂��_���܂����I");
            UpdateFireEffect();
            OnBrazierStateChanged?.Invoke(this); // �C�x���g�𔭉�
        }
    }

    public void ExtinguishBrazier()
    {
        if (isLit)
        {
            isLit = false;
            Debug.Log($"{name} �̉΂������܂����B");
            UpdateFireEffect();
            OnBrazierStateChanged?.Invoke(this); // �C�x���g�𔭉�
        }
    }

    private void UpdateFireEffect()
    {
        if (currentFlameEffect != null)
        {
            currentFlameEffect.SetActive(isLit);
        }
    }

    // ���Α�̕\��/��\���𐧌� (BrazierSpawner���g�p)
    public void SetActiveState(bool active)
    {
        gameObject.SetActive(active);
    }
}

