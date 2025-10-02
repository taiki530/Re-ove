using UnityEngine;
using System.Collections.Generic;
using System.Linq; // for .All()

public class PressurePlate : MonoBehaviour
{
    [Tooltip("���̃X�C�b�`�����֘A�t�����Ă���BrazierSpawner�̃��X�g")]
    public List<BrazierSpawner> brazierSpawners;

    [Tooltip("���̃X�C�b�`�������䂷��h�A�̃��X�g")]
    public List<Door> controlledDoors; // ���䂷��h�A�̃��X�g

    private int collisionCount = 0; // ����ł���I�u�W�F�N�g�̐�

    public bool IsCurrentlyPressed => collisionCount > 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TimeLoopPlayer"))
        {
            collisionCount++;
            if (collisionCount == 1) // ���߂ē��܂ꂽ��
            {
                Debug.Log($"[{name}] ���܂ꂽ�I�֘A���鐹�Α���o�������܂��B");
                ActivateBraziers();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TimeLoopPlayer"))
        {
            collisionCount--;
            if (collisionCount == 0) // �S�ẴI�u�W�F�N�g�����ꂽ��
            {
                Debug.Log($"[{name}] ���ꂽ�I�֘A���鐹�Α���\��/�j�����܂��B");
                DeactivateBraziers();
            }
        }
    }

    private void ActivateBraziers()
    {
        foreach (var spawner in brazierSpawners)
        {
            if (spawner != null)
            {
                spawner.SpawnBrazier(); // BrazierSpawner�ɐ��Α�̐������˗�
            }
        }
    }

    private void DeactivateBraziers()
    {
        bool isTimeLoopActive = TimeLoopManager.Instance != null ? TimeLoopManager.Instance.isTimeLoopActive : false;

        foreach (var spawner in brazierSpawners)
        {
            if (spawner != null)
            {
                // �^�C�����[�v���łȂ� ���� �΂�������Ă��Ȃ����Α�̂ݔj��
                if (!isTimeLoopActive && !spawner.IsBrazierLit())
                {
                    spawner.DespawnBrazier(); // �΂�������Ă��Ȃ����Α�͔j��
                }
                // �^�C�����[�v���̏ꍇ�A�΂�������Ă��Ȃ����PressurePlate��ResetPlate()�ł܂Ƃ߂Ĕj������邽�߁A�����ł͉������Ȃ�
                // �΂�������Ă��鐹�Α�́A�^�C�����[�v�����ǂ����ɂ�����炸���̂܂܎c��
            }
        }
    }

    // ���Α�̓_����Ԃ��`�F�b�N���A�h�A���J���郁�\�b�h
    public void CheckAndOpenDoors()
    {
        // �֘A����BrazierSpawner�̃��X�g����łȂ���΃`�F�b�N
        if (brazierSpawners == null || brazierSpawners.Count == 0)
        {
            Debug.LogWarning($"[{name}] BrazierSpawner���ݒ肳��Ă��܂���B�h�A���J�������������܂���B");
            return;
        }

        // �S�Ă̊֘A���鐹�Α䂪�_�����Ă��邩�`�F�b�N
        bool allBraziersLit = brazierSpawners.All(spawner => spawner != null && spawner.IsBrazierLit());

        if (allBraziersLit)
        {
            Debug.Log($"[{name}] �S�Ă̊֘A���鐹�Α�ɉ΂�������܂����I�h�A���J���܂��I");
            foreach (var door in controlledDoors)
            {
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }
        else
        {
            Debug.Log($"[{name}] �܂��S�Ă̊֘A���鐹�Α�ɉ΂�������Ă��܂���B");
        }
    }

    // ���Z�b�g�����i�^�C�����[�v���j
    public void ResetPlate()
    {
        Debug.Log($"[{name}] �X�C�b�`�������Z�b�g��...");

        foreach (var spawner in brazierSpawners)
        {
            if (spawner != null)
            {
                spawner.ResetSpawner(); // BrazierSpawner�ɐ��Α�̔j�����˗�
            }
        }

        collisionCount = 0; // ����ł���I�u�W�F�N�g�̃J�E���g�����Z�b�g

        // �h�A��BrazierManager�ł͂Ȃ�PressurePlate�ŊǗ�����邽�߁A�����ł̓h�A����鏈���͍s��Ȃ�
        // �h�A�̕��鏈���́A�^�C�����[�v�̃��Z�b�g����TimeLoopManager���璼�ڍs���邩�A
        // ���邢��Door�X�N���v�g���̂����Z�b�g���Ɏ����ŕ���悤�Ɏ�������Ă���ׂ�
        foreach (var door in controlledDoors)
        {
            if (door != null)
            {
                //door.CloseDoor(); // �h�A�����
            }
        }

        Debug.Log($"[{name}] �X�C�b�`�������Z�b�g���܂����B");
    }

    public void ClearRecord()
    {
        // �����X�|�[�����W�b�N���Ȃ����߁A�����ł̓��ʂȏ����͕s�v
    }
}
