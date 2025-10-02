using UnityEngine;

public class BrazierSpawner : MonoBehaviour
{
    [Tooltip("���̃X�|�i�[���������鐹�Α��Prefab")]
    public GameObject brazierPrefab;

    private GameObject spawnedBrazierGameObject; // ���ݐ�������Ă��鐹�Α��GameObject
    private Brazier brazierComponent;             // �������ꂽ���Α��Brazier�R���|�[�l���g

    // ���C���_�� ���ԋL�^���폜���A�v���C���[���ߋ��ɓ_���������ǂ����̃t���O�ɕύX
    private bool wasLitByPlayerInPreviousLoop = false; // �ߋ��̃��[�v�Ńv���C���[�ɂ���ē_�����ꂽ���Ƃ����邩

    void Awake()
    {
        Debug.Log($"[BrazierSpawner] {name}: Awake ���Ăяo����܂����B�������: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");
    }

    /// ���̃X�|�i�[�̈ʒu�ɐ��Α�𐶐����܂��B
    public void SpawnBrazier()
    {
        Debug.Log($"[BrazierSpawner] {name}: SpawnBrazier() ���Ăяo����܂����B���݂̃X�|�i�[�̏��: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

        if (spawnedBrazierGameObject == null)
        {
            // �X�|�i�[���g�̈ʒu�Ɖ�]�Ő��Α�𐶐�
            spawnedBrazierGameObject = Instantiate(brazierPrefab, transform.position, transform.rotation);
            brazierComponent = spawnedBrazierGameObject.GetComponent<Brazier>();

            if (brazierComponent != null)
            {
                // �������ꂽ���Α�ɋL�^�l��n��
                brazierComponent.ExternalWasLitByPlayerInPreviousLoop = wasLitByPlayerInPreviousLoop;
                Debug.Log($"[BrazierSpawner] {brazierComponent.name} �ɋL�^�l��n���܂���: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

                // �������ꂽ���Α��BrazierManager�ɓo�^
                BrazierManager.Instance?.RegisterBrazier(brazierComponent);
                Debug.Log($"[BrazierSpawner] ���Α�𐶐����܂���: {spawnedBrazierGameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[BrazierSpawner] �������ꂽ�I�u�W�F�N�g {spawnedBrazierGameObject.name} �� Brazier �R���|�[�l���g��������܂���B");
            }
        }
        else
        {
            // ���ɐ�������Ă���ꍇ�́A��\����Ԃ���\����Ԃɖ߂�
            if (!spawnedBrazierGameObject.activeSelf)
            {
                spawnedBrazierGameObject.SetActive(true);
                Debug.Log($"[BrazierSpawner] ���Α���ĕ\�����܂���: {spawnedBrazierGameObject.name}");
            }
        }
    }

    /// �������ꂽ���Α��j�����܂��B
    public void DespawnBrazier()
    {
        if (spawnedBrazierGameObject != null)
        {
            // BrazierManager����o�^������
            if (brazierComponent != null)
            {
                BrazierManager.Instance?.UnregisterBrazier(brazierComponent); // UnregisterBrazier ���Ăяo��
            }
            Destroy(spawnedBrazierGameObject);
            spawnedBrazierGameObject = null;
            brazierComponent = null;
            Debug.Log("[BrazierSpawner] ���Α��j�����܂����B");
        }
    }

    /// �������ꂽ���Α䂪���݉΂�������Ă��邩���擾���܂��B
    /// <returns>�΂�������Ă����true�A�����łȂ����false�B</returns>
    public bool IsBrazierLit()
    {
        return brazierComponent != null && brazierComponent.isLit;
    }

    /// �X�|�i�[�����Z�b�g���A�������ꂽ���Α���i�΂��t���Ă��Ă��j�j�����܂��B
    /// ����͎�Ƀ^�C�����[�v�̃��Z�b�g���Ɏg�p����B
    public void ResetSpawner()
    {
        Debug.Log($"[BrazierSpawner] {name}: ResetSpawner() ���Ăяo����܂����B�X�V�O: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

        // wasLitByPlayerInPreviousLoop �� NotifyPlayerLitBrazier() �� true �ɐݒ肳��A
        // ClearRecord() ���Ă΂��܂� true �̂܂ܕێ������B
        // �����ł͏�Ԃ�ύX���Ȃ��B

        DespawnBrazier(); // ���Α��j��
        Debug.Log($"[BrazierSpawner] �X�|�i�[�����Z�b�g���܂���: {gameObject.name}�B�X�V��: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");
    }

    // ���ǉ����\�b�h�� Brazier����v���C���[���_���������Ƃ�ʒm�����
    public void NotifyPlayerLitBrazier()
    {
        wasLitByPlayerInPreviousLoop = true;
        Debug.Log($"[BrazierSpawner] {name}: �v���C���[�����Α�ɉ΂𓔂��܂����BwasLitByPlayerInPreviousLoop �� True �ɐݒ肵�܂����B");
    }

    // �^�C�����[�v�̃��Z�b�g���� BrazierSpawner ���L�^���N���A���郁�\�b�h
    public void ClearRecord()
    {
        wasLitByPlayerInPreviousLoop = false; // �S�Ă̋L�^���N���A
        Debug.Log($"[BrazierSpawner] {name}: �L�^���N���A���܂����BwasLitByPlayerInPreviousLoop �� False �Ƀ��Z�b�g�B");
    }

    // �ߋ��ɓ_�����ꂽ�L�^�����邩���擾���郁�\�b�h
    public bool GetWasLitByPlayerInPreviousLoop()
    {
        return wasLitByPlayerInPreviousLoop;
    }
}