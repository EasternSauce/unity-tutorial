using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public bool HasItemOnCursor => selectedItemController != null && selectedItemController.HasItem;
    public InventoryGridHandler GridHandler => gridHandler;
    public SelectedItemController SelectedItemController => selectedItemController;
    public ItemHighlightController ItemHighlightController => itemHighlightController;
    public Vector2 MousePosition { get; set; }

    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
    [SerializeField] private ItemHighlightController itemHighlightController;
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private CharacterDefeatHandler defeatHandler;

    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    public EquipmentItemSlot SelectedItemSlot
    {
        get => selectedItemSlot;
        set => selectedItemSlot = value;
    }

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            itemHighlightController?.SetCurrentGrid(value);
            gridHandler?.SetCurrentGrid(value);
        }
    }

    private void Awake()
    {
        if (selectedItemController == null)
            selectedItemController = FindFirstObjectByType<SelectedItemController>();

        if (gridHandler == null)
            gridHandler = FindFirstObjectByType<InventoryGridHandler>();

        if (defeatHandler == null)
            defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();

        if (mouseInput == null)
            mouseInput = FindFirstObjectByType<MouseInput>();
    }

    public void InsertRandomItem()
    {
        if (selectedItemGrid == null || selectedItemController == null) return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItemController.SelectedItem;
        selectedItemController.ClearSelectedItem();
        gridHandler?.InsertItem(selectedItemGrid, itemToInsert);
    }

    private void CreateRandomItem()
    {
        if (selectedItemController.HasItem || itemDatas == null || itemDatas.Count == 0) return;

        int selectedItemId = Random.Range(0, itemDatas.Count);
        InventoryItem newItem = CreateNewInventoryItem(itemDatas[selectedItemId]);
        selectedItemController.SetSelectedItem(newItem);
        itemHighlightController?.SetSelectedItem(newItem);
    }

    public InventoryItem CreateNewInventoryItem(ItemData itemData)
    {
        if (inventoryItemPrefab == null || targetCanvas == null) return null;

        GameObject newItemGO = Instantiate(inventoryItemPrefab, targetCanvas);
        InventoryItem newInventoryItem = newItemGO.GetComponent<InventoryItem>();
        RectTransform newItemRectTransform = newItemGO.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);
        newInventoryItem?.Set(itemData);
        return newInventoryItem;
    }

    public void ThrowItemOnGround()
    {
        if (selectedItemController == null) return;

        InventoryItem itemToDrop = selectedItemController.Drop();
        if (itemToDrop != null)
            DropItem(GameManager.instance.playerObject.transform.position, itemToDrop);
    }

    public void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return;
        ItemSpawnManager.instance.SpawnItem(dropPosition, itemToDrop.itemData);
        Destroy(itemToDrop.gameObject);
    }

    public void ItemSlotInput()
    {
        if (selectedItemController == null || selectedItemSlot == null) return;

        if (!selectedItemController.HasItem)
        {
            InventoryItem item = selectedItemSlot.PickUpItem();
            if (item != null)
            {
                selectedItemController.PickUp(item);
                itemHighlightController?.SetSelectedItem(item);
            }
        }
        else
        {
            PlaceItemIntoSlot();
        }
    }

    private void PlaceItemIntoSlot()
    {
        if (selectedItemController == null || selectedItemSlot == null) return;

        if (!selectedItemSlot.Check(selectedItemController.SelectedItem)) return;

        InventoryItem replacedItem = selectedItemSlot.ReplaceItem(selectedItemController.SelectedItem);
        if (replacedItem == null)
        {
            selectedItemController.ClearSelectedItem();
            itemHighlightController?.SetSelectedItem(null);
        }
        else
        {
            selectedItemController.SetSelectedItem(replacedItem);
            itemHighlightController?.SetSelectedItem(replacedItem);
        }
    }
}
