using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName; // �A�C�e���̖��O
    public Sprite itemIcon; // �C���x���g���ɕ\������A�C�R��
    public GameObject itemPrefabToSpawn; // �g�p���ɐ�������3D�I�u�W�F�N�g��Prefab

    void OnTriggerEnter(Collider other)
    {
        // �v���C���[���G�ꂽ�ꍇ
        if (other.CompareTag("Player"))
        {
            // InventoryManager��Prefab����n��
            InventoryManager.Instance.AddItem(itemName, itemIcon, itemPrefabToSpawn);
            Destroy(gameObject);
        }
        // �N���[���iTimeLoopPlayer�j���G�ꂽ�ꍇ
        else if (other.CompareTag("TimeLoopPlayer"))
        {
            // �N���[�����������E�����ꍇ�ł��A�v���C���[�̃C���x���g���ɒǉ�����
            if (itemName == "Torch") // �����iTorch�j�Ƃ������O�̃A�C�e���̏ꍇ
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(itemName, itemIcon, itemPrefabToSpawn); // �v���C���[�̃C���x���g���ɒǉ�
                    Debug.Log($"�N���[�������� ({itemName}) ���E���܂����B�v���C���[�̃C���x���g���ɒǉ�����܂����B");
                    Destroy(gameObject); // �A�C�e���͏��ł�����
                }
                else
                {
                    Debug.LogWarning("InventoryManager.Instance ��������܂���B�N���[�����������E���܂���ł����B");
                }
            }
        }
    }
}