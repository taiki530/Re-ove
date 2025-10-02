using UnityEngine;

public class BrazierSpawner : MonoBehaviour
{
    [Tooltip("このスポナーが生成する聖火台のPrefab")]
    public GameObject brazierPrefab;

    private GameObject spawnedBrazierGameObject; // 現在生成されている聖火台のGameObject
    private Brazier brazierComponent;             // 生成された聖火台のBrazierコンポーネント

    // ★修正点★ 時間記録を削除し、プレイヤーが過去に点灯したかどうかのフラグに変更
    private bool wasLitByPlayerInPreviousLoop = false; // 過去のループでプレイヤーによって点灯されたことがあるか

    void Awake()
    {
        Debug.Log($"[BrazierSpawner] {name}: Awake が呼び出されました。初期状態: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");
    }

    /// このスポナーの位置に聖火台を生成します。
    public void SpawnBrazier()
    {
        Debug.Log($"[BrazierSpawner] {name}: SpawnBrazier() が呼び出されました。現在のスポナーの状態: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

        if (spawnedBrazierGameObject == null)
        {
            // スポナー自身の位置と回転で聖火台を生成
            spawnedBrazierGameObject = Instantiate(brazierPrefab, transform.position, transform.rotation);
            brazierComponent = spawnedBrazierGameObject.GetComponent<Brazier>();

            if (brazierComponent != null)
            {
                // 生成された聖火台に記録値を渡す
                brazierComponent.ExternalWasLitByPlayerInPreviousLoop = wasLitByPlayerInPreviousLoop;
                Debug.Log($"[BrazierSpawner] {brazierComponent.name} に記録値を渡しました: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

                // 生成された聖火台をBrazierManagerに登録
                BrazierManager.Instance?.RegisterBrazier(brazierComponent);
                Debug.Log($"[BrazierSpawner] 聖火台を生成しました: {spawnedBrazierGameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[BrazierSpawner] 生成されたオブジェクト {spawnedBrazierGameObject.name} に Brazier コンポーネントが見つかりません。");
            }
        }
        else
        {
            // 既に生成されている場合は、非表示状態から表示状態に戻す
            if (!spawnedBrazierGameObject.activeSelf)
            {
                spawnedBrazierGameObject.SetActive(true);
                Debug.Log($"[BrazierSpawner] 聖火台を再表示しました: {spawnedBrazierGameObject.name}");
            }
        }
    }

    /// 生成された聖火台を破棄します。
    public void DespawnBrazier()
    {
        if (spawnedBrazierGameObject != null)
        {
            // BrazierManagerから登録を解除
            if (brazierComponent != null)
            {
                BrazierManager.Instance?.UnregisterBrazier(brazierComponent); // UnregisterBrazier を呼び出す
            }
            Destroy(spawnedBrazierGameObject);
            spawnedBrazierGameObject = null;
            brazierComponent = null;
            Debug.Log("[BrazierSpawner] 聖火台を破棄しました。");
        }
    }

    /// 生成された聖火台が現在火が灯されているかを取得します。
    /// <returns>火が灯されていればtrue、そうでなければfalse。</returns>
    public bool IsBrazierLit()
    {
        return brazierComponent != null && brazierComponent.isLit;
    }

    /// スポナーをリセットし、生成された聖火台を（火が付いていても）破棄します。
    /// これは主にタイムループのリセット時に使用する。
    public void ResetSpawner()
    {
        Debug.Log($"[BrazierSpawner] {name}: ResetSpawner() が呼び出されました。更新前: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");

        // wasLitByPlayerInPreviousLoop は NotifyPlayerLitBrazier() で true に設定され、
        // ClearRecord() が呼ばれるまで true のまま保持される。
        // ここでは状態を変更しない。

        DespawnBrazier(); // 聖火台を破棄
        Debug.Log($"[BrazierSpawner] スポナーをリセットしました: {gameObject.name}。更新後: wasLitByPlayerInPreviousLoop={wasLitByPlayerInPreviousLoop}");
    }

    // ★追加メソッド★ Brazierからプレイヤーが点灯したことを通知される
    public void NotifyPlayerLitBrazier()
    {
        wasLitByPlayerInPreviousLoop = true;
        Debug.Log($"[BrazierSpawner] {name}: プレイヤーが聖火台に火を灯しました。wasLitByPlayerInPreviousLoop を True に設定しました。");
    }

    // タイムループのリセット時に BrazierSpawner が記録をクリアするメソッド
    public void ClearRecord()
    {
        wasLitByPlayerInPreviousLoop = false; // 全ての記録をクリア
        Debug.Log($"[BrazierSpawner] {name}: 記録をクリアしました。wasLitByPlayerInPreviousLoop を False にリセット。");
    }

    // 過去に点灯された記録があるかを取得するメソッド
    public bool GetWasLitByPlayerInPreviousLoop()
    {
        return wasLitByPlayerInPreviousLoop;
    }
}