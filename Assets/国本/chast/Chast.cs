using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Tooltip("�󔠂Ɋi�[�����A�C�e���iItemHolder�j")]
    public ItemHolder[] storedItems;

    public void Open()
    {
        foreach (var itemHolder in storedItems)
        {
            if (itemHolder != null)
            {
                Debug.Log($"�󔠂̒��� {itemHolder.itemData.itemName} �������Ă����I");
                // �����Ńv���C���[�ɓn�������Ȃ�
            }
        }
    }

    void Update()
    {
        // K�L�[����������󔠂̒��g�����O�ɕ\��
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
//    [Tooltip("���̕󔠂ɓ����Ă���A�C�e���i�C���X�y�N�^�[����ݒ�j")]
//    public List<Item> storedItems;

//    private bool isOpened = false;

//    public void OpenChest()
//    {
//        if (isOpened)
//        {
//            Debug.Log("���̕󔠂͊��ɊJ����Ă��܂��B");
//            return;
//        }

//        isOpened = true;

//        Debug.Log(" �󔠂��J���܂����I�ȉ��̃A�C�e�����擾���܂����F");

//        foreach (var item in storedItems)
//        {
//            Debug.Log($" {item.itemName} (ID: {item.itemID})");
//        }

//        storedItems.Clear(); // ���g�����
//    }
//}
