using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//アイテムデータ
public class PlayerItemData
{
    public string id;   //アイテムid

    private int count;  //所持数

    //コンストラクタ
    public PlayerItemData(string id, int count = 1)
    {
        this.id = id;
        this.count = count;
    }

    //所持数カウントアップ
    public void CountUp(int value = 1)
    {
        count += value;
    }

    //所持数カウントダウン
    public void CountDown(int value = 1)
    {
        count -= value;
    }
}

public class PlayerItemManager : MonoBehaviour
{
    [SerializeField] private List<ItemSourceData> _itemSourceDataList;  //アイテムソースリスト

    private List<PlayerItemData> _playerItemDataList = new List<PlayerItemData>();  //プレイヤーの所持アイテム


    private void Awake()
    {
        LoadItemSourceData();
    }

    //アイテムをロードする
    private void LoadItemSourceData()
    {
        _itemSourceDataList = Resources.LoadAll("ScriptableObject", typeof(ItemSourceData)).Cast<ItemSourceData>().ToList();
    }

    //アイテムソースデータを取得
    public ItemSourceData GetItemSourceData(string id)
    {
        //アイテムを検索
        foreach (var sourceData in _itemSourceDataList)
        {
            //IDが一致していたら
            if (sourceData.id == id)
            {
                return sourceData;
            }
        }
        return null;
    }


    //アイテムを取得
    public void CountItem(string itemId, int count)
    {
        for (int i = 0; i < _playerItemDataList.Count; i++)
        {
            //IDが一致していたらカウント
            if (_playerItemDataList[i].id == itemId)
            {
                _playerItemDataList[i].CountUp(count);
                break;
            }
        }

        //IDが一致しなければアイテムを追加
        PlayerItemData itemData = new PlayerItemData(itemId, count);
        _playerItemDataList.Add(itemData);
    }

    //アイテムを使用
    public void UseItem(string itemId, int count)
    {
        //List内を検索
        for (int i = 0; i < _playerItemDataList.Count; i++)
        {
            //IDが一致していたらカウント
            if (_playerItemDataList[i].id == itemId)
            {
                //アイテムをカウントダウン
                _playerItemDataList[i].CountDown(count);
                break;
            }
        }
    }
}
