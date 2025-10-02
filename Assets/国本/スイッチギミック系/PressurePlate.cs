using UnityEngine;
using System.Collections.Generic;
using System.Linq; // for .All()

public class PressurePlate : MonoBehaviour
{
    [Tooltip("このスイッチ床が関連付けられているBrazierSpawnerのリスト")]
    public List<BrazierSpawner> brazierSpawners;

    [Tooltip("このスイッチ床が制御するドアのリスト")]
    public List<Door> controlledDoors; // 制御するドアのリスト

    private int collisionCount = 0; // 踏んでいるオブジェクトの数

    public bool IsCurrentlyPressed => collisionCount > 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TimeLoopPlayer"))
        {
            collisionCount++;
            if (collisionCount == 1) // 初めて踏まれた時
            {
                Debug.Log($"[{name}] 踏まれた！関連する聖火台を出現させます。");
                ActivateBraziers();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TimeLoopPlayer"))
        {
            collisionCount--;
            if (collisionCount == 0) // 全てのオブジェクトが離れた時
            {
                Debug.Log($"[{name}] 離れた！関連する聖火台を非表示/破棄します。");
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
                spawner.SpawnBrazier(); // BrazierSpawnerに聖火台の生成を依頼
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
                // タイムループ中でない かつ 火が灯されていない聖火台のみ破棄
                if (!isTimeLoopActive && !spawner.IsBrazierLit())
                {
                    spawner.DespawnBrazier(); // 火が灯されていない聖火台は破棄
                }
                // タイムループ中の場合、火が灯されていなければPressurePlateのResetPlate()でまとめて破棄されるため、ここでは何もしない
                // 火が灯されている聖火台は、タイムループ中かどうかにかかわらずそのまま残す
            }
        }
    }

    // 聖火台の点灯状態をチェックし、ドアを開けるメソッド
    public void CheckAndOpenDoors()
    {
        // 関連するBrazierSpawnerのリストが空でなければチェック
        if (brazierSpawners == null || brazierSpawners.Count == 0)
        {
            Debug.LogWarning($"[{name}] BrazierSpawnerが設定されていません。ドアを開ける条件がありません。");
            return;
        }

        // 全ての関連する聖火台が点灯しているかチェック
        bool allBraziersLit = brazierSpawners.All(spawner => spawner != null && spawner.IsBrazierLit());

        if (allBraziersLit)
        {
            Debug.Log($"[{name}] 全ての関連する聖火台に火が灯されました！ドアを開けます！");
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
            Debug.Log($"[{name}] まだ全ての関連する聖火台に火が灯されていません。");
        }
    }

    // リセット処理（タイムループ時）
    public void ResetPlate()
    {
        Debug.Log($"[{name}] スイッチ床をリセット中...");

        foreach (var spawner in brazierSpawners)
        {
            if (spawner != null)
            {
                spawner.ResetSpawner(); // BrazierSpawnerに聖火台の破棄を依頼
            }
        }

        collisionCount = 0; // 踏んでいるオブジェクトのカウントをリセット

        // ドアはBrazierManagerではなくPressurePlateで管理されるため、ここではドアを閉じる処理は行わない
        // ドアの閉じる処理は、タイムループのリセット時にTimeLoopManagerから直接行われるか、
        // あるいはDoorスクリプト自体がリセット時に自動で閉じるように実装されているべき
        foreach (var door in controlledDoors)
        {
            if (door != null)
            {
                //door.CloseDoor(); // ドアを閉じる
            }
        }

        Debug.Log($"[{name}] スイッチ床をリセットしました。");
    }

    public void ClearRecord()
    {
        // 自動スポーンロジックがないため、ここでの特別な処理は不要
    }
}
