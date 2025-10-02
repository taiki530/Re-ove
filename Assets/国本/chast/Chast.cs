using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Tooltip("宝箱に格納されるアイテム（ItemHolder）")]
    public ItemHolder[] storedItems;

    public void Open()
    {
        foreach (var itemHolder in storedItems)
        {
            if (itemHolder != null)
            {
                Debug.Log($"宝箱の中に {itemHolder.itemData.itemName} が入っていた！");
                // ここでプレイヤーに渡す処理など
            }
        }
    }

    void Update()
    {
        // Kキーを押したら宝箱の中身をログに表示
        if (Input.GetKeyDown(KeyCode.K))
        {
            Open();
        }
    }
}



//using System.Collections.Generic;
//using UnityEngine;

//public class Chest : MonoBehaviour
//{
//    [Tooltip("この宝箱に入っているアイテム（インスペクターから設定）")]
//    public List<Item> storedItems;

//    private bool isOpened = false;

//    public void OpenChest()
//    {
//        if (isOpened)
//        {
//            Debug.Log("この宝箱は既に開かれています。");
//            return;
//        }

//        isOpened = true;

//        Debug.Log(" 宝箱を開けました！以下のアイテムを取得しました：");

//        foreach (var item in storedItems)
//        {
//            Debug.Log($" {item.itemName} (ID: {item.itemID})");
//        }

//        storedItems.Clear(); // 中身を空に
//    }
//}
