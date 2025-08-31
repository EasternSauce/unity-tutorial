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
    private GameObject selectedItemParentGameObject;

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
    public SelectedItemController SelectedItemController => selectedItemController;

    private void Awake()
    {
        CreateSelectedItemParentIfMissing();
    }

    private void CreateSelectedItemParentIfMissing()
    {
        if (selectedItemParentGameObject == null)
        {
            selectedItemParentGameObject = GameObject.Find("SelectedItemContainer");
            if (selectedItemParentGameObject == null)
            {
                selectedItemParentGameObject = new GameObject("SelectedItemContainer");
                if (targetCanvas != null)
                    selectedItemParentGameObject.transform.SetParent(targetCanvas, false);
            }
        }
    }

    private void ParentSelectedItem(InventoryItem item)
    {
        if (item == null) return;
        CreateSelectedItemParentIfMissing();
        if (item.transform.parent != selectedItemParentGameObject.transform)
            item.transform.SetParent(selectedItemParentGameObject.transform, false);
    }

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
        if (selectedItemController.HasItem)
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
        ParentSelectedItem(newInventoryItem);
        return newInventoryItem;
    }

    private void LateUpdate()
    {
        if (selectedItemController != null && selectedItemController.HasItem)
        {
            ParentSelectedItem(selectedItemController.SelectedItem);
        }
    }
}
