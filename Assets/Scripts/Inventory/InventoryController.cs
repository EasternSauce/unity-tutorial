using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private InventoryItemHighlightController itemHighlightController;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
    [SerializeField] private PanelManager uiPanelManager;

    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;
    private GameObject selectedItemParentGameObject;

    public EquipmentItemSlot SelectedItemSlot { get => selectedItemSlot; set => selectedItemSlot = value; }
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            if (gridHandler != null) gridHandler.SetCurrentGrid(value);
            if (itemHighlightController != null && gridHandler != null && gridHandler.IsMouseOverGrid(Input.mousePosition))
            {
                itemHighlightController.SetCurrentGrid(value);
            }
        }
    }

    public bool HasItemOnCursor => selectedItemController.HasItem;
    public SelectedItemController SelectedItemController => selectedItemController;

    private void Awake()
    {
        if (uiPanelManager == null) uiPanelManager = FindFirstObjectByType<PanelManager>();
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
                if (targetCanvas != null) selectedItemParentGameObject.transform.SetParent(targetCanvas, false);
            }
        }
    }

    private void ParentSelectedItem(InventoryItem item)
    {
        if (item == null || !selectedItemController.HasItem) return;
        CreateSelectedItemParentIfMissing();
        if (item.transform.parent != selectedItemParentGameObject.transform)
            item.transform.SetParent(selectedItemParentGameObject.transform, false);
    }

    public void HandlePrimaryClick(Vector2 mousePosition)
    {
        if (!uiPanelManager || !uiPanelManager.IsInventoryOpen) return;

        if (selectedItemGrid != null)
        {
            if (gridHandler.IsMouseOverGrid(mousePosition))
                gridHandler.HandleClick(selectedItemGrid, mousePosition);
            return;
        }

        if (selectedItemSlot != null)
        {
            selectedItemSlot.HandleClick(selectedItemController);
            return;
        }

        if (selectedItemController.HasItem && !selectedItemController.SelectedItem.IsEquipped && !IsPointerOverUI(mousePosition))
        {
            ItemDropUtility.ThrowItemOnGround(selectedItemController);
        }
    }

    public void ThrowItemOnGround()
    {
        if (!uiPanelManager || !uiPanelManager.IsInventoryOpen) return;
        if (selectedItemController.HasItem && !selectedItemController.SelectedItem.IsEquipped)
            ItemDropUtility.ThrowItemOnGround(selectedItemController);
    }

    public void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        if (itemToDrop != null && !itemToDrop.IsEquipped)
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
            if (!selectedItemController.SelectedItem.IsEquipped)
                ParentSelectedItem(selectedItemController.SelectedItem);
            else
                selectedItemController.ClearSelectedItem();
        }

        if (selectedItemGrid != null && itemHighlightController != null && gridHandler.IsMouseOverGrid(Input.mousePosition))
        {
            itemHighlightController.SetCurrentGrid(selectedItemGrid);
        }
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;
        var eventData = new PointerEventData(EventSystem.current) { position = screenPosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
