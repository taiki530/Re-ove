using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BrazierManager : MonoBehaviour
{
    public static BrazierManager Instance { get; private set; }

    public List<Brazier> braziers = new List<Brazier>(); // 管理する聖火台のリスト

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
            Debug.Log($"Brazier: {brazier.name} がBrazierManagerに登録されました。現在 {braziers.Count} 個の聖火台を管理中。");
            brazier.OnBrazierStateChanged += HandleBrazierStateChanged; // ★追加点★ イベント購読
        }
    }

    public void UnregisterBrazier(Brazier brazier)
    {
        if (braziers.Contains(brazier))
        {
            braziers.Remove(brazier);
            Debug.Log($"Brazier: {brazier.name} がBrazierManagerから登録解除されました。現在 {braziers.Count} 個の聖火台を管理中。");
            brazier.OnBrazierStateChanged -= HandleBrazierStateChanged; // イベント購読解除
        }
    }

    // Brazierの点灯状態変化を処理するメソッド
    private void HandleBrazierStateChanged(Brazier changedBrazier)
    {
        Debug.Log($"Brazier {changedBrazier.name} の状態が変更されました (isLit: {changedBrazier.isLit})。");
        // ここで全てのPressurePlateに通知し、それぞれの条件をチェックさせる
        // シーン内の全てのPressurePlateを見つけて、CheckAndOpenDoorsを呼び出す
        // より効率的な方法として、BrazierSpawnerがPressurePlateに自身の状態を通知することも考えられる
        // 今回はシンプルに、BrazierManagerが全てのPressurePlateに通知する
        foreach (var plate in FindObjectsOfType<PressurePlate>())
        {
            plate.CheckAndOpenDoors();
        }
    }

    // ★削除点★ CheckAllBraziersLit() からドアを開けるロジックを削除
    // このメソッドはもはや直接ドアを開けません。PressurePlateがその責任を負います。
    public void CheckAllBraziersLit()
    {
        // 無効な（Destroyされた）Brazierをリストから除去
        braziers.RemoveAll(b => b == null);

        // Debug.Log("BrazierManager: 全ての聖火台の点灯状態をチェックしました。");
        // ドアを開けるロジックはPressurePlateに移管されたため、ここでは何もしない
    }

    // タイムループのリセット時にBrazierManagerのリストをクリーンアップ
    public void ResetAllBraziers()
    {
        // BrazierSpawnerがBrazierの生成と破棄を管理するため、
        // ここでは単にリストからnullになったBrazierを削除する
        braziers.RemoveAll(b => b == null);
        Debug.Log("[BrazierManager] 管理リストをリセットしました。");
    }

    public void ClearAllBrazierRecords()
    {
        // 記録ロジックがないため、ここでの特別な処理は不要
    }
}