using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//アイテムの種類(列挙体)
public enum ItemType
{
    THROWING = 0,   //投擲
    RECOVERY = 1,   //回復
}

//アイテムのソースデータ
[CreateAssetMenu(menuName = "BlogAssets/ItemSourceData")]
public class ItemSourceData : ScriptableObject
{
    //アイテム識別用id
    [SerializeField] private string _id;
    //idを取得
    public string id
    {
        get { return _id; }
    }

    //アイテムの名前
    [SerializeField] private string _itemName;
    //アイテム名を取得
    public string itemName
    {
        get { return _itemName; }
    }

    //アイテムの種類
    [SerializeField] private ItemType _itemType;
    //アイテムの種類を取得
    public ItemType itemType
    {
        get { return _itemType; }
    }

    //アイテムの見た目
    [SerializeField] private Sprite _sprite;
    //アイテムの見た目を取得
    public Sprite sprite
    {
        get { return _sprite; }
    }
}