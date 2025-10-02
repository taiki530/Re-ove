using UnityEngine;

// �������V�[���ɑ��݂���Ƃ��̐U�镑�����`
public class Torch : MonoBehaviour
{
    // ���ǉ��ӏ��� �΂̃G�t�F�N�g�𐶐�����ʒu��Transform
    [Tooltip("�΂̃G�t�F�N�g�𐶐�����ʒu��Transform�������ɐݒ肵�܂��B")]
    public Transform flameSpawnPoint;

    // �΂̃G�t�F�N�g�ȂǁA�������R���Ă����Ԃ�����GameObject
    public GameObject flameEffectPrefab; // ���C���ӏ��� Prefab�ɖ��O��ύX

    // ���ǉ��ӏ��� �������ꂽ�G�t�F�N�g�̃C���X�^���X��ێ�
    private GameObject currentFlameEffect;

    public bool IsLit { get; private set; } = false; // �΂��_���Ă��邩

    // ���������A�΂͏����Ă����Ԃɂ���
    void Awake()
    {
        // ���C���ӏ��� Awake()�ł̏��������W�b�N��ύX
        if (flameEffectPrefab != null && flameSpawnPoint != null)
        {
            // flameSpawnPoint�̈ʒu�ɃG�t�F�N�g�𐶐����A�q�Ƃ��Đݒ�
            Vector3 targetDirection = Vector3.up; // ������Ɍ������
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            if (flameSpawnPoint != null)
            {
                currentFlameEffect = Instantiate(flameEffectPrefab, flameSpawnPoint.position, targetRotation, flameSpawnPoint);
            }
            else
            {
                currentFlameEffect = Instantiate(flameEffectPrefab, transform.position, targetRotation, transform);
            }
            //currentFlameEffect = Instantiate(flameEffectPrefab, flameSpawnPoint.position, Quaternion.identity, flameSpawnPoint);
            currentFlameEffect.SetActive(true); // ������Ԃ͔�\��
        }
        else
        {
            Debug.LogWarning("Torch: �΂̃G�t�F�N�g��Prefab�܂��͐����|�C���g���ݒ肳��Ă��܂���B", this);
        }
    }

    // �����ɉ΂�_���郁�\�b�h
    public void LightTorch()
    {
        if (!IsLit)
        {
            IsLit = true;
            if (currentFlameEffect != null)
            {
                currentFlameEffect.SetActive(true); // �΂̃G�t�F�N�g��L����
            }
            Debug.Log($"{name} �ɉ΂��_���܂����I");
        }
    }

    // �����̉΂��������\�b�h�i���Z�b�g�p�j
    public void ExtinguishTorch()
    {
        if (IsLit)
        {
            IsLit = false;
            if (currentFlameEffect != null)
            {
                currentFlameEffect.SetActive(false); // �΂̃G�t�F�N�g�𖳌���
            }
            Debug.Log($"{name} �̉΂������܂����B");
        }
    }

    // �^�C�����[�v�̃��Z�b�g�ɑΉ����邽�߂̃C���^�[�t�F�[�X�i�I�v�V�����j
    // Seedling��ResetVines�̂悤�Ȏd�g�݂ɍ��킹��ꍇ
    public void ResetState()
    {
        ExtinguishTorch();
    }
}

