using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�A�C�e���̎��(�񋓑�)
public enum ItemType
{
    THROWING = 0,   //����
    RECOVERY = 1,   //��
}

//�A�C�e���̃\�[�X�f�[�^
[CreateAssetMenu(menuName = "BlogAssets/ItemSourceData")]
public class ItemSourceData : ScriptableObject
{
    //�A�C�e�����ʗpid
    [SerializeField] private string _id;
    //id���擾
    public string id
    {
        get { return _id; }
    }

    //�A�C�e���̖��O
    [SerializeField] private string _itemName;
    //�A�C�e�������擾
    public string itemName
    {
        get { return _itemName; }
    }

    //�A�C�e���̎��
    [SerializeField] private ItemType _itemType;
    //�A�C�e���̎�ނ��擾
    public ItemType itemType
    {
        get { return _itemType; }
    }

    //�A�C�e���̌�����
    [SerializeField] private Sprite _sprite;
    //�A�C�e���̌����ڂ��擾
    public Sprite sprite
    {
        get { return _sprite; }
    }
}