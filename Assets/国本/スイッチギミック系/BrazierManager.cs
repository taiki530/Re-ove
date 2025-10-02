using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BrazierManager : MonoBehaviour
{
    public static BrazierManager Instance { get; private set; }

    public List<Brazier> braziers = new List<Brazier>(); // �Ǘ����鐹�Α�̃��X�g

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterBrazier(Brazier brazier)
    {
        if (!braziers.Contains(brazier))
        {
            braziers.Add(brazier);
            Debug.Log($"Brazier: {brazier.name} ��BrazierManager�ɓo�^����܂����B���� {braziers.Count} �̐��Α���Ǘ����B");
            brazier.OnBrazierStateChanged += HandleBrazierStateChanged; // ���ǉ��_�� �C�x���g�w��
        }
    }

    public void UnregisterBrazier(Brazier brazier)
    {
        if (braziers.Contains(brazier))
        {
            braziers.Remove(brazier);
            Debug.Log($"Brazier: {brazier.name} ��BrazierManager����o�^��������܂����B���� {braziers.Count} �̐��Α���Ǘ����B");
            brazier.OnBrazierStateChanged -= HandleBrazierStateChanged; // �C�x���g�w�ǉ���
        }
    }

    // Brazier�̓_����ԕω����������郁�\�b�h
    private void HandleBrazierStateChanged(Brazier changedBrazier)
    {
        Debug.Log($"Brazier {changedBrazier.name} �̏�Ԃ��ύX����܂��� (isLit: {changedBrazier.isLit})�B");
        // �����őS�Ă�PressurePlate�ɒʒm���A���ꂼ��̏������`�F�b�N������
        // �V�[�����̑S�Ă�PressurePlate�������āACheckAndOpenDoors���Ăяo��
        // �������I�ȕ��@�Ƃ��āABrazierSpawner��PressurePlate�Ɏ��g�̏�Ԃ�ʒm���邱�Ƃ��l������
        // ����̓V���v���ɁABrazierManager���S�Ă�PressurePlate�ɒʒm����
        foreach (var plate in FindObjectsOfType<PressurePlate>())
        {
            plate.CheckAndOpenDoors();
        }
    }

    // ���폜�_�� CheckAllBraziersLit() ����h�A���J���郍�W�b�N���폜
    // ���̃��\�b�h�͂��͂Ⓖ�ڃh�A���J���܂���BPressurePlate�����̐ӔC�𕉂��܂��B
    public void CheckAllBraziersLit()
    {
        // �����ȁiDestroy���ꂽ�jBrazier�����X�g���珜��
        braziers.RemoveAll(b => b == null);

        // Debug.Log("BrazierManager: �S�Ă̐��Α�̓_����Ԃ��`�F�b�N���܂����B");
        // �h�A���J���郍�W�b�N��PressurePlate�Ɉڊǂ��ꂽ���߁A�����ł͉������Ȃ�
    }

    // �^�C�����[�v�̃��Z�b�g����BrazierManager�̃��X�g���N���[���A�b�v
    public void ResetAllBraziers()
    {
        // BrazierSpawner��Brazier�̐����Ɣj�����Ǘ����邽�߁A
        // �����ł͒P�Ƀ��X�g����null�ɂȂ���Brazier���폜����
        braziers.RemoveAll(b => b == null);
        Debug.Log("[BrazierManager] �Ǘ����X�g�����Z�b�g���܂����B");
    }

    public void ClearAllBrazierRecords()
    {
        // �L�^���W�b�N���Ȃ����߁A�����ł̓��ʂȏ����͕s�v
    }
}