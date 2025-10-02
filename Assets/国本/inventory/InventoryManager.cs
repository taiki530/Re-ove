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
        public GameObject itemPrefab; // 使用時に生成する3DオブジェクトのPrefab
    }

    public List<InventoryItemData> inventory = new List<InventoryItemData>();

    [SerializeField] private int maxInventorySlots = 8;

    public GameObject inventoryUIObject; // インベントリUI全体 (Canvas)
    public Image selectionHighlight; // 選択枠のUI Image

    public Image currentItemIconImage; // 現在選択されているアイテムのアイコン用Image
    public TextMeshProUGUI currentItemNameText; // 現在選択されているアイテムの名前用Text
    public TextMeshProUGUI currentItemCountText; // 現在選択されているアイテムの数量用Text

    public RectTransform itemContainer; // アイテムスロットの親 (ItemContainer) のRectTransform
    public GameObject itemSlotPrefab; // 個々のアイテムスロットUI要素のPrefab

    private int selectedSlotIndex = 0; // 現在選択されているインベントリスロットのインデックス

    public Transform playerTransform; // プレイヤーのTransform
    public float itemSpawnDistance = 2f; // アイテムを生成するプレイヤーからの距離

    // タイムループ開始時の松明の数量を記録する変数
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
            Debug.Log($"1つの{itemName}を追加しました。現在の数量: {existingItem.quantity}");
        }
        else
        {
            if (inventory.Count < maxInventorySlots)
            {
                inventory.Add(new InventoryItemData { itemName = itemName, itemIcon = itemIcon, quantity = 1, itemPrefab = itemPrefab });
                Debug.Log($"新しいアイテムを追加しました: {itemName}");
            }
            else
            {
                Debug.Log("インベントリがいっぱいです！");
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
            Debug.LogWarning($"{itemName}を削除しようとしましたが、インベントリに見つかりませんでした。");
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
            Debug.LogWarning("現在のアイテム表示UI要素が割り当てられていません。");
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
                        Debug.Log($"{itemToUse.itemName}を使用しました。Cubeを配置しました。");
                        RemoveItemAt(selectedSlotIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"{itemToUse.itemName}のPrefabまたはプレイヤートランスフォームが割り当てられていません。");
                    }
                    break;
                case "Potion":
                    Debug.Log("ポーションを使用しました。プレイヤーが回復しました！");
                    RemoveItemAt(selectedSlotIndex);
                    break;
                case "Torch":
                    if (itemToUse.itemPrefab != null && playerTransform != null)
                    {
                        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * itemSpawnDistance;
                        GameObject spawnedTorch = Instantiate(itemToUse.itemPrefab, spawnPosition, Quaternion.identity);
                        Debug.Log($"{itemToUse.itemName}を使用しました。松明を配置しました。");
                        RemoveItemAt(selectedSlotIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"{itemToUse.itemName}のPrefabまたはプレイヤートランスフォームが割り当てられていません。");
                    }
                    break;
                default:
                    Debug.Log($"{itemToUse.itemName}を使用しました。(特定の用途は定義されていません)");
                    break;
            }
        }
        else
        {
            Debug.Log("アイテムが選択されていないか、スロットが空です。");
        }
    }

    public bool HasItem(string itemName)
    {
        return inventory.Any(item => item.itemName == itemName && item.quantity > 0);
    }

    // ★修正点★ タイムループ開始時の松明の数量を保存 (引数なし)
    public void SaveInitialTorchQuantity()
    {
        InventoryItemData torchItem = inventory.FirstOrDefault(item => item.itemName == "Torch");
        _initialTorchQuantityInLoop = (torchItem != null) ? torchItem.quantity : 0;
        Debug.Log($"[InventoryManager] タイムループ開始時の松明の数量を保存しました: {_initialTorchQuantityInLoop}");
    }

    // ★追加メソッド★ タイムループ時に松明の数量を復元
    public void RestoreTorchesToInitialQuantity()
    {
        InventoryItemData currentTorchItem = inventory.FirstOrDefault(item => item.itemName == "Torch");
        int currentTorchQuantity = (currentTorchItem != null) ? currentTorchItem.quantity : 0;

        if (currentTorchQuantity < _initialTorchQuantityInLoop)
        {
            int quantityToAdd = _initialTorchQuantityInLoop - currentTorchQuantity;
            // 松明のPrefabとSpriteをどこかから取得する必要がある
            // ここでは、現在のインベントリに松明が存在すればその情報を使用
            if (currentTorchItem != null)
            {
                for (int i = 0; i < quantityToAdd; i++)
                {
                    AddItem("Torch", currentTorchItem.itemIcon, currentTorchItem.itemPrefab);
                }
            }
            else // インベントリに松明が全くない場合（全て使い切った後など）
            {
                // ここでデフォルトの松明のSpriteとPrefabをどう取得するかは、ゲームの設計による
                // 例: InventoryManagerにPublic変数でデフォルトの松明PrefabとSpriteへの参照を持たせる
                // public Sprite defaultTorchIcon;
                // public GameObject defaultTorchPrefab;
                // AddItem("Torch", defaultTorchIcon, defaultTorchPrefab);
                Debug.LogWarning("[InventoryManager] 松明の復元に失敗しました: デフォルトの松明情報が不足しています。");
            }
            Debug.Log($"[InventoryManager] 松明の数量を {_initialTorchQuantityInLoop} に復元しました。");
        }
        else if (currentTorchQuantity > _initialTorchQuantityInLoop)
        {
            int quantityToRemove = currentTorchQuantity - _initialTorchQuantityInLoop;
            for (int i = 0; i < quantityToRemove; i++)
            {
                RemoveItem("Torch");
            }
            Debug.Log($"[InventoryManager] 松明の数量を {_initialTorchQuantityInLoop} に調整しました（減らしました）。");
        }
        else
        {
            Debug.Log("[InventoryManager] 松明の数量は変更されていませんでした。");
        }
        UpdateInventoryUI();
        UpdateCurrentSelectedItemDisplay();
    }
}
