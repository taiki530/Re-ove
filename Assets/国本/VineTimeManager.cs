using UnityEngine;
using System.Collections.Generic; // For List

public class VineTimeManager : MonoBehaviour
{
    public static VineTimeManager Instance { get; private set; }

    private float startTime;

    public List<Seedling> seedlings = new List<Seedling>();
    public List<PressurePlate> pressurePlates = new List<PressurePlate>(); // PressurePlate�ւ̎Q��
    public BrazierManager brazierManager; // BrazierManager�ւ̎Q��

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // StartTimer���\�b�h���g��
    public void StartTimer()
    {
        startTime = Time.time;
        Debug.Log($"[Timer] �J�n����: {startTime}");

        // �^�C�����[�v�J�n���A�ߋ��̋L�^�Ɋ�Â��Ċe�M�~�b�N���������s����悤�X�P�W���[��
        // ��PressurePlate��Brazier�̎����N�����W�b�N�͍폜��
        foreach (var seedling in seedlings)
        {
            seedling.ScheduleAutoGrow(startTime);
        }
    }

    public float GetStartTime() => startTime;

    // �^�C�����[�v�̃��Z�b�g����
    public void ResetTimeLoop()
    {
        Debug.Log("[TimeLoop] �^�C�����[�v�����Z�b�g���܂��B");

        // �e�M�~�b�N�̏�Ԃ����Z�b�g
        foreach (var seedling in seedlings)
        {
            seedling.ResetVines();
        }
        foreach (var plate in pressurePlates) // PressurePlate�̃��Z�b�g���Ăяo��
        {
            plate.ResetPlate();
        }
        // BrazierManager��PressurePlate��Brazier���Ǘ����邽�߁A���ړI��ResetAllBraziers�͕s�v�ɂȂ邪�A
        // �O�̂��ߎc���Ă������APressurePlate��ResetPlate()�ɔC���邩������
        // brazierManager?.ResetAllBraziers(); // �K�v�ł����
    }

    // �^�C�����[�v�̋L�^�����S�ɃN���A���鏈��
    public void ClearAllRecords()
    {
        Debug.Log("[TimeLoop] �S�Ẵ^�C�����[�v�L�^���N���A���܂��B");

        foreach (var seedling in seedlings)
        {
            seedling.ClearRecord();
        }
        foreach (var plate in pressurePlates)
        {
            plate.ClearRecord();
        }
        // brazierManager?.ClearAllBrazierRecords(); // �K�v�ł����
    }

    // �V�[�����̃I�u�W�F�N�g�������Ŏ擾����ꍇ�̗�
    void OnEnable()
    {
        // �V�[������Seedling, PressurePlate, BrazierManager�������Ŏ擾
        seedlings = new List<Seedling>(FindObjectsOfType<Seedling>());
        pressurePlates = new List<PressurePlate>(FindObjectsOfType<PressurePlate>());
        brazierManager = FindObjectOfType<BrazierManager>();
    }
}
