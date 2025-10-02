using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string itemName;  // �A�C�e���̖��O
        //public Sprite icon;      // UI�ɕ\������A�C�R��
        public GameObject itemPrefab; // �Q�[�����ɑ��݂���3D�I�u�W�F�N�g
    }

    public List<Item> items = new List<Item>();  // �A�C�e���̃��X�g

    // �A�C�e�����C���x���g���ɒǉ�
    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        Debug.Log(newItem.itemName + "���C���x���g���ɒǉ����܂����B");
    }

    // �A�C�e�����C���x���g������폜
    public void RemoveItem(Item itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
            Debug.Log(itemToRemove.itemName + "���C���x���g������폜���܂����B");
        }
        else
        {
            Debug.Log("�A�C�e����������܂���ł����B");
        }
    }

    // ����̃A�C�e��������
    public Item FindItem(string itemName)
    {
        return items.Find(item => item.itemName == itemName);
    }



    // �A�C�e�����g�p����i��: �L�[���g���j
    public bool UseKey(string keyName)
    {
        Item keyItem = FindItem(keyName);
        if (keyItem != null)
        {
            RemoveItem(keyItem); // �A�C�e�����C���x���g������폜
            return true; // �L�[���g�������Ƃ�����
        }
        return false; // �L�[�������Ă��Ȃ�
    }
}
