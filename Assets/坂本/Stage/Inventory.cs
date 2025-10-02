using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string itemName;  // アイテムの名前
        //public Sprite icon;      // UIに表示するアイコン
        public GameObject itemPrefab; // ゲーム内に存在する3Dオブジェクト
    }

    public List<Item> items = new List<Item>();  // アイテムのリスト

    // アイテムをインベントリに追加
    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        Debug.Log(newItem.itemName + "をインベントリに追加しました。");
    }

    // アイテムをインベントリから削除
    public void RemoveItem(Item itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
            Debug.Log(itemToRemove.itemName + "をインベントリから削除しました。");
        }
        else
        {
            Debug.Log("アイテムが見つかりませんでした。");
        }
    }

    // 特定のアイテムを検索
    public Item FindItem(string itemName)
    {
        return items.Find(item => item.itemName == itemName);
    }



    // アイテムを使用する（例: キーを使う）
    public bool UseKey(string keyName)
    {
        Item keyItem = FindItem(keyName);
        if (keyItem != null)
        {
            RemoveItem(keyItem); // アイテムをインベントリから削除
            return true; // キーを使ったことを示す
        }
        return false; // キーを持っていない
    }
}
