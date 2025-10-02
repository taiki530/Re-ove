using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class InventoryItemData
    {
        public string itemName;
        public Sprite itemIcon;
        public int quantity;
        public int maxStackSize = 99;
        public GameObject itemPrefab; // �g�p���ɐ�������3D�I�u�W�F�N�g��Prefab
    }

    public List<InventoryItemData> inventory = new List<InventoryItemData>();

    [SerializeField] private int maxInventorySlots = 8;

    public GameObject inventoryUIObject; // �C���x���g��UI�S�� (Canvas)
    public Image selectionHighlight; // �I��g��UI Image

    public Image currentItemIconImage; // ���ݑI������Ă���A�C�e���̃A�C�R���pImage
    public TextMeshProUGUI currentItemNameText; // ���ݑI������Ă���A�C�e���̖��O�pText
    public TextMeshProUGUI currentItemCountText; // ���ݑI������Ă���A�C�e���̐��ʗpText

    public RectTransform itemContainer; // �A�C�e���X���b�g�̐e (ItemContainer) ��RectTransform
    public GameObject itemSlotPrefab; // �X�̃A�C�e���X���b�gUI�v�f��Prefab

    private int selectedSlotIndex = 0; // ���ݑI������Ă���C���x���g���X���b�g�̃C���f�b�N�X

    public Transform playerTransform; // �v���C���[��Transform
    public float itemSpawnDistance = 2f; // �A�C�e���𐶐�����v���C���[����̋���

    // �^�C�����[�v�J�n���̏����̐��ʂ��L�^����ϐ�
    private int _initialTorchQuantityInLoop = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (inventoryUIObject != null)
        {
            inventoryUIObject.SetActive(false);
        }

        if (selectionHighlight != null)
        {
            selectionHighlight.gameObject.SetActive(false);
        }

        SetupItemSlotsUI();
        UpdateInventoryUI();
        UpdateCurrentSelectedItemDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryUI();
        }

        if (inventoryUIObject.activeSelf)
        {
            for (int i = 0; i < maxInventorySlots; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectSlot(i);
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                UseSelectedItem();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                int direction = (int)Mathf.Sign(Input.mouseScrollDelta.y);
                selectedSlotIndex -= direction;

                if (selectedSlotIndex < 0)
                {
                    selectedSlotIndex = maxInventorySlots - 1;
                }
                else if (selectedSlotIndex >= maxInventorySlots)
                {
                    selectedSlotIndex = 0;
                }
                UpdateSelectionHighlight();
                UpdateCurrentSelectedItemDisplay();
            }
        }
    }

    void SetupItemSlotsUI()
    {
        if (itemContainer == null || itemSlotPrefab == null)
        {
            return;
        }

        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, itemContainer);
            slotGO.name = "ItemSlot_" + (i + 1);
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUIObject != null)
        {
            bool isActive = !inventoryUIObject.activeSelf;
            inventoryUIObject.SetActive(isActive);

            if (selectionHighlight != null)
            {
                selectionHighlight.gameObject.SetActive(isActive);
                if (isActive)
                {
                    UpdateSelectionHighlight();
                    UpdateCurrentSelectedItemDisplay();
                }
                else
                {
                    ClearCurrentSelectedItemDisplay();
                }
            }
        }
    }

    public void AddItem(string itemName, Sprite itemIcon, GameObject itemPrefab = null)
    {
        InventoryItemData existingItem = inventory.FirstOrDefault(item => item.itemName == itemName && item.quantity < item.maxStackSize);

        if (existingItem != null)
        {
            existingItem.quantity++;
            Debug.Log($"1��{itemName}��ǉ����܂����B���݂̐���: {existingItem.quantity}");
        }
        else
        {
            if (inventory.Count < maxInventorySlots)
            {
                inventory.Add(new InventoryItemData { itemName = itemName, itemIcon = itemIcon, quantity = 1, itemPrefab = itemPrefab });
                Debug.Log($"�V�����A�C�e����ǉ����܂���: {itemName}");
            }
            else
            {
                Debug.Log("�C���x���g���������ς��ł��I");
                return;
            }
        }
        UpdateInventoryUI();
        UpdateCurrentSelectedItemDisplay();
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        InventoryItemData itemToRemove = inventory.FirstOrDefault(item => item.itemName == itemName);

        if (itemToRemove != null)
        {
            int index = inventory.IndexOf(itemToRemove);
            RemoveItemAt(index, amount);
        }
        else
        {
            Debug.LogWarning($"{itemName}���폜���悤�Ƃ��܂������A�C���x���g���Ɍ�����܂���ł����B");
        }
    }

    private void RemoveItemAt(int index, int amount = 1)
    {
        if (index >= 0 && index < inventory.Count)
        {
            inventory[index].quantity -= amount;
            if (inventory[index].quantity <= 0)
            {
                inventory.RemoveAt(index);
                if (selectedSlotIndex >= inventory.Count && inventory.Count > 0)
                {
                    selectedSlotIndex = inventory.Count - 1;
                }
                else if (inventory.Count == 0)
                {
                    selectedSlotIndex = 0;
                }
            }
            UpdateInventoryUI();
            UpdateSelectionHighlight();
            UpdateCurrentSelectedItemDisplay();
        }
    }

    void UpdateInventoryUI()
    {
        if (itemContainer == null) return;

        Image[] slotImages = itemContainer.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] slotQuantityTexts = itemContainer.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < maxInventorySlots; i++)
        {
            if (i < slotImages.Length)
            {
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0);
            }
            if (i < slotQuantityTexts.Length)
            {
                slotQuantityTexts[i].text = "";
            }
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            if (i < slotImages.Length)
            {
                slotImages[i].sprite = inventory[i].itemIcon;
                slotImages[i].color = Color.white;

                if (inventory[i].quantity > 1)
                {
                    if (i < slotQuantityTexts.Length)
                    {
                        slotQuantityTexts[i].text = inventory[i].quantity.ToString();
                    }
                }
                else
                {
                    if (i < slotQuantityTexts.Length)
                    {
                        slotQuantityTexts[i].text = "";
                    }
                }
            }
        }
        UpdateSelectionHighlight();
    }

    void SelectSlot(int index)
    {
        if (index >= 0 && index < maxInventorySlots)
        {
            selectedSlotIndex = index;
            UpdateSelectionHighlight();
            UpdateCurrentSelectedItemDisplay();
        }
    }

    void UpdateSelectionHighlight()
    {
        if (selectionHighlight == null || itemContainer == null) return;

        Image[] slotImages = itemContainer.GetComponentsInChildren<Image>();

        if (selectedSlotIndex >= 0 && selectedSlotIndex < slotImages.Length)
        {
            selectionHighlight.rectTransform.position = slotImages[selectedSlotIndex].rectTransform.position;
            selectionHighlight.gameObject.SetActive(true);
        }
        else
        {
            selectionHighlight.gameObject.SetActive(false);
        }
    }

    void UpdateCurrentSelectedItemDisplay()
    {
        if (currentItemIconImage == null || currentItemNameText == null || currentItemCountText == null)
        {
            Debug.LogWarning("���݂̃A�C�e���\��UI�v�f�����蓖�Ă��Ă��܂���B");
            return;
        }

        if (selectedSlotIndex >= 0 && selectedSlotIndex < inventory.Count)
        {
            InventoryItemData currentItem = inventory[selectedSlotIndex];
            currentItemIconImage.sprite = currentItem.itemIcon;
            currentItemIconImage.color = Color.white;
            currentItemNameText.text = currentItem.itemName;
            currentItemCountText.text = currentItem.quantity > 1 ? currentItem.quantity.ToString() : "";
        }
        else
        {
            ClearCurrentSelectedItemDisplay();
        }
    }

    void ClearCurrentSelectedItemDisplay()
    {
        if (currentItemIconImage != null)
        {
            currentItemIconImage.sprite = null;
            currentItemIconImage.color = new Color(1, 1, 1, 0);
        }
        if (currentItemNameText != null)
        {
            currentItemNameText.text = "";
        }
        if (currentItemCountText != null)
        {
            currentItemCountText.text = "";
        }
    }

    void UseSelectedItem()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < inventory.Count)
        {
            InventoryItemData itemToUse = inventory[selectedSlotIndex];

            switch (itemToUse.itemName)
            {
                case "Cube":
                    if (itemToUse.itemPrefab != null && playerTransform != null)
                    {
                        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * itemSpawnDistance;
                        spawnPosition.y += itemToUse.itemPrefab.transform.localScale.y / 2;
                        Instantiate(itemToUse.itemPrefab, spawnPosition, Quaternion.identity);
                        Debug.Log($"{itemToUse.itemName}���g�p���܂����BCube��z�u���܂����B");
                        RemoveItemAt(selectedSlotIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"{itemToUse.itemName}��Prefab�܂��̓v���C���[�g�����X�t�H�[�������蓖�Ă��Ă��܂���B");
                    }
                    break;
                case "Potion":
                    Debug.Log("�|�[�V�������g�p���܂����B�v���C���[���񕜂��܂����I");
                    RemoveItemAt(selectedSlotIndex);
                    break;
                case "Torch":
                    if (itemToUse.itemPrefab != null && playerTransform != null)
                    {
                        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * itemSpawnDistance;
                        GameObject spawnedTorch = Instantiate(itemToUse.itemPrefab, spawnPosition, Quaternion.identity);
                        Debug.Log($"{itemToUse.itemName}���g�p���܂����B������z�u���܂����B");
                        RemoveItemAt(selectedSlotIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"{itemToUse.itemName}��Prefab�܂��̓v���C���[�g�����X�t�H�[�������蓖�Ă��Ă��܂���B");
                    }
                    break;
                default:
                    Debug.Log($"{itemToUse.itemName}���g�p���܂����B(����̗p�r�͒�`����Ă��܂���)");
                    break;
            }
        }
        else
        {
            Debug.Log("�A�C�e�����I������Ă��Ȃ����A�X���b�g����ł��B");
        }
    }

    public bool HasItem(string itemName)
    {
        return inventory.Any(item => item.itemName == itemName && item.quantity > 0);
    }

    // ���C���_�� �^�C�����[�v�J�n���̏����̐��ʂ�ۑ� (�����Ȃ�)
    public void SaveInitialTorchQuantity()
    {
        InventoryItemData torchItem = inventory.FirstOrDefault(item => item.itemName == "Torch");
        _initialTorchQuantityInLoop = (torchItem != null) ? torchItem.quantity : 0;
        Debug.Log($"[InventoryManager] �^�C�����[�v�J�n���̏����̐��ʂ�ۑ����܂���: {_initialTorchQuantityInLoop}");
    }

    // ���ǉ����\�b�h�� �^�C�����[�v���ɏ����̐��ʂ𕜌�
    public void RestoreTorchesToInitialQuantity()
    {
        InventoryItemData currentTorchItem = inventory.FirstOrDefault(item => item.itemName == "Torch");
        int currentTorchQuantity = (currentTorchItem != null) ? currentTorchItem.quantity : 0;

        if (currentTorchQuantity < _initialTorchQuantityInLoop)
        {
            int quantityToAdd = _initialTorchQuantityInLoop - currentTorchQuantity;
            // ������Prefab��Sprite���ǂ�������擾����K�v������
            // �����ł́A���݂̃C���x���g���ɏ��������݂���΂��̏����g�p
            if (currentTorchItem != null)
            {
                for (int i = 0; i < quantityToAdd; i++)
                {
                    AddItem("Torch", currentTorchItem.itemIcon, currentTorchItem.itemPrefab);
                }
            }
            else // �C���x���g���ɏ������S���Ȃ��ꍇ�i�S�Ďg���؂�����Ȃǁj
            {
                // �����Ńf�t�H���g�̏�����Sprite��Prefab���ǂ��擾���邩�́A�Q�[���̐݌v�ɂ��
                // ��: InventoryManager��Public�ϐ��Ńf�t�H���g�̏���Prefab��Sprite�ւ̎Q�Ƃ���������
                // public Sprite defaultTorchIcon;
                // public GameObject defaultTorchPrefab;
                // AddItem("Torch", defaultTorchIcon, defaultTorchPrefab);
                Debug.LogWarning("[InventoryManager] �����̕����Ɏ��s���܂���: �f�t�H���g�̏�����񂪕s�����Ă��܂��B");
            }
            Debug.Log($"[InventoryManager] �����̐��ʂ� {_initialTorchQuantityInLoop} �ɕ������܂����B");
        }
        else if (currentTorchQuantity > _initialTorchQuantityInLoop)
        {
            int quantityToRemove = currentTorchQuantity - _initialTorchQuantityInLoop;
            for (int i = 0; i < quantityToRemove; i++)
            {
                RemoveItem("Torch");
            }
            Debug.Log($"[InventoryManager] �����̐��ʂ� {_initialTorchQuantityInLoop} �ɒ������܂����i���炵�܂����j�B");
        }
        else
        {
            Debug.Log("[InventoryManager] �����̐��ʂ͕ύX����Ă��܂���ł����B");
        }
        UpdateInventoryUI();
        UpdateCurrentSelectedItemDisplay();
    }
}
