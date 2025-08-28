using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private ItemHighlightController itemHighlightController;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;

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

    public bool HasItemOnCursor => selectedItemController.HasItem;

    public void HandlePrimaryClick(Vector2 mousePosition)
    {
        if (selectedItemGrid != null)
        {
            gridHandler.HandleClick(selectedItemGrid, mousePosition, selectedItemController, itemHighlightController);
            return;
        }

        if (selectedItemSlot != null)
        {
            selectedItemSlot.HandleClick(selectedItemController, itemHighlightController);
            return;
        }

        if (selectedItemController.HasItem && !UIUtility.IsPointerOverUI(mousePosition))
        {
            ItemDropUtility.ThrowItemOnGround(selectedItemController);
        }
    }

    public void ThrowItemOnGround()
    {
        ItemDropUtility.ThrowItemOnGround(selectedItemController);
    }

    public void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        ItemDropUtility.DropItem(dropPosition, itemToDrop);
    }

    public InventoryItem CreateNewInventoryItem(ItemData itemData)
    {
        if (inventoryItemPrefab == null || targetCanvas == null) return null;
        GameObject newItemGameObject = Instantiate(inventoryItemPrefab, targetCanvas);
        InventoryItem newInventoryItem = newItemGameObject.GetComponent<InventoryItem>();
        RectTransform newItemRectTransform = newItemGameObject.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);
        newInventoryItem?.Set(itemData);
        return newInventoryItem;
    }
}
