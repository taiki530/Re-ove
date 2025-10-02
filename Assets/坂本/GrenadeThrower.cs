using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    // �O���l�[�h�A�C�e���̖��O (InventoryManager�œo�^����Ă��閼�O�ƈ�v������)
    public string grenadeItemName = "Grenade";

    void Update()
    {
        // G�L�[�������ꂽ�Ƃ�
        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            // PlayerStatus������ꍇ�i�v���C���[������ł��Ȃ����m�F�j
            PlayerStatus player = FindObjectOfType<PlayerStatus>();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (player != null && player.IsDead() == true)
            {
                return; // �v���C���[�����S���Ă���ꍇ�͏������Ȃ�
            }

            //ThrowGrenade();

            // �C���x���g���}�l�[�W���[������������Ă��邩�m�F
            if (InventoryManager.Instance != null)
            {
                // �C���x���g���Ɏw��̃O���l�[�h�A�C�e�������邩�A�����ʂ�1�ȏォ�m�F
                InventoryManager.InventoryItemData grenadeInInventory =
                    InventoryManager.Instance.inventory.Find(item => item.itemName == grenadeItemName && item.quantity >= 1);

                if (grenadeInInventory != null)
                {
                    // �O���l�[�h�𓊂���
                    ThrowGrenade();
                    // �C���x���g������O���l�[�h��1���炷
                    InventoryManager.Instance.RemoveItem(grenadeItemName);

                    playerController.IsThrowing(120);
                }
                else
                {
                    Debug.Log("�O���l�[�h������܂���I");
                }
            }
            else
            {
                Debug.LogWarning("InventoryManager ���V�[���Ɍ�����܂���B");
            }
        }
    }

    void ThrowGrenade()
    {
        // �O���l�[�h�𐶐�
        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ���������ɗ͂�������
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
        }
    }
}