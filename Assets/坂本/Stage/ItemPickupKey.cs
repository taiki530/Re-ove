using UnityEngine;

public class ItemPickupKey : MonoBehaviour
{
    [SerializeField] string itemName; // �A�C�e���̖��O
    [SerializeField] float maxPickupDistance = 5f; // �A�C�e�����E����ő勗��
    //[SerializeField] GameObject keyImage; // UI�ɕ\������A�C�e���̉摜
    //[SerializeField] Sprite itemIcon; // �A�C�e���̃A�C�R���iUI�p�j
    [SerializeField] GameObject itemPrefab; // �A�C�e����3D���f��
    [SerializeField] AudioClip ItemPickupSound; // ���ʉ��̂��߂�AudioClip

    public Inventory playerInventory; // �v���C���[�̃C���x���g��
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource��ǉ�
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // �J�����̑O����Ray�𔭎�
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            // Ray���I�u�W�F�N�g�ɓ��������ꍇ�A���w��̋������ɂ���ꍇ
            if (Physics.Raycast(ray, out hit, maxPickupDistance))
            {
                if (hit.collider.gameObject == gameObject) // �N���b�N�����I�u�W�F�N�g�����̃A�C�e�����m�F
                {
                    PickupItem();

                    //keyImage.SetActive(true);
                }
            }
        }
    }

    void PickupItem()
    {
        Debug.Log("Picked up: " + itemName);

        // �V�����A�C�e�����쐬���A�C���x���g���ɒǉ�
        Inventory.Item newItem = new Inventory.Item
        {
            itemName = itemName,
            //icon = itemIcon,
            itemPrefab = itemPrefab
        };

        playerInventory.AddItem(newItem); // �C���x���g���ɃA�C�e����ǉ�
        audioSource.PlayOneShot(ItemPickupSound); //���ʉ����Đ�
        //Destroy(gameObject); // �A�C�e���I�u�W�F�N�g���V�[������폜


    }
}
