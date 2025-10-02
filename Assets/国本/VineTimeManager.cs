using UnityEngine;
using System.Collections.Generic; // For List

public class VineTimeManager : MonoBehaviour
{
    public static VineTimeManager Instance { get; private set; }

    private float startTime;

    public List<Seedling> seedlings = new List<Seedling>();
    public List<PressurePlate> pressurePlates = new List<PressurePlate>(); // PressurePlateへの参照
    public BrazierManager brazierManager; // BrazierManagerへの参照

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // StartTimerメソッドを拡張
    public void StartTimer()
    {
        startTime = Time.time;
        Debug.Log($"[Timer] 開始時間: {startTime}");

        // タイムループ開始時、過去の記録に基づいて各ギミックを自動実行するようスケジュール
        // ★PressurePlateとBrazierの自動起動ロジックは削除★
        foreach (var seedling in seedlings)
        {
            seedling.ScheduleAutoGrow(startTime);
        }
    }

    public float GetStartTime() => startTime;

    // タイムループのリセット処理
    public void ResetTimeLoop()
    {
        Debug.Log("[TimeLoop] タイムループをリセットします。");

        // 各ギミックの状態をリセット
        foreach (var seedling in seedlings)
        {
            seedling.ResetVines();
        }
        foreach (var plate in pressurePlates) // PressurePlateのリセットを呼び出す
        {
            plate.ResetPlate();
        }
        // BrazierManagerはPressurePlateがBrazierを管理するため、直接的なResetAllBraziersは不要になるが、
        // 念のため残しておくか、PressurePlateのResetPlate()に任せるかを検討
        // brazierManager?.ResetAllBraziers(); // 必要であれば
    }

    // タイムループの記録を完全にクリアする処理
    public void ClearAllRecords()
    {
        Debug.Log("[TimeLoop] 全てのタイムループ記録をクリアします。");

        foreach (var seedling in seedlings)
        {
            seedling.ClearRecord();
        }
        foreach (var plate in pressurePlates)
        {
            plate.ClearRecord();
        }
        // brazierManager?.ClearAllBrazierRecords(); // 必要であれば
    }

    // シーン内のオブジェクトを自動で取得する場合の例
    void OnEnable()
    {
        // シーン内のSeedling, PressurePlate, BrazierManagerを自動で取得
        seedlings = new List<Seedling>(FindObjectsOfType<Seedling>());
        pressurePlates = new List<PressurePlate>(FindObjectsOfType<PressurePlate>());
        brazierManager = FindObjectOfType<BrazierManager>();
    }
}
