using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    // グレネードアイテムの名前 (InventoryManagerで登録されている名前と一致させる)
    public string grenadeItemName = "Grenade";

    void Update()
    {
        // Gキーが押されたとき
        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            // PlayerStatusがある場合（プレイヤーが死んでいないか確認）
            PlayerStatus player = FindObjectOfType<PlayerStatus>();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (player != null && player.IsDead() == true)
            {
                return; // プレイヤーが死亡している場合は処理しない
            }

            //ThrowGrenade();

            // インベントリマネージャーが初期化されているか確認
            if (InventoryManager.Instance != null)
            {
                // インベントリに指定のグレネードアイテムがあるか、かつ数量が1以上か確認
                InventoryManager.InventoryItemData grenadeInInventory =
                    InventoryManager.Instance.inventory.Find(item => item.itemName == grenadeItemName && item.quantity >= 1);

                if (grenadeInInventory != null)
                {
                    // グレネードを投げる
                    ThrowGrenade();
                    // インベントリからグレネードを1つ減らす
                    InventoryManager.Instance.RemoveItem(grenadeItemName);

                    playerController.IsThrowing(120);
                }
                else
                {
                    Debug.Log("グレネードがありません！");
                }
            }
            else
            {
                Debug.LogWarning("InventoryManager がシーンに見つかりません。");
            }
        }
    }

    void ThrowGrenade()
    {
        // グレネードを生成
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 投擲方向に力を加える
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}