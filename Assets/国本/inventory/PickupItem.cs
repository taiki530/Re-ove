using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName; // アイテムの名前
    public Sprite itemIcon; // インベントリに表示するアイコン
    public GameObject itemPrefabToSpawn; // 使用時に生成する3DオブジェクトのPrefab

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーが触れた場合
        if (other.CompareTag("Player"))
        {
            // InventoryManagerにPrefab情報を渡す
            InventoryManager.Instance.AddItem(itemName, itemIcon, itemPrefabToSpawn);
            Destroy(gameObject);
        }
        // クローン（TimeLoopPlayer）が触れた場合
        else if (other.CompareTag("TimeLoopPlayer"))
        {
            // クローンが松明を拾った場合でも、プレイヤーのインベントリに追加する
            if (itemName == "Torch") // 松明（Torch）という名前のアイテムの場合
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(itemName, itemIcon, itemPrefabToSpawn); // プレイヤーのインベントリに追加
                    Debug.Log($"クローンが松明 ({itemName}) を拾いました。プレイヤーのインベントリに追加されました。");
                    Destroy(gameObject); // アイテムは消滅させる
                }
                else
                {
                    Debug.LogWarning("InventoryManager.Instance が見つかりません。クローンが松明を拾えませんでした。");
                }
            }
        }
    }
}