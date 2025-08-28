using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{
    public bool HasItemOnCursor => selectedItemController.HasItem;
    public InventoryGridHandler GridHandler => gridHandler;
    public SelectedItemController SelectedItemController => selectedItemController;
    public ItemHighlightController ItemHighlightController => itemHighlightController;

    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
    [SerializeField] private ItemHighlightController itemHighlightController;
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private CharacterDefeatHandler defeatHandler;

    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    public EquipmentItemSlot SelectedItemSlot { get => selectedItemSlot; set => selectedItemSlot = value; }
    public ItemGrid SelectedItemGrid { get => selectedItemGrid; set { selectedItemGrid = value; itemHighlightController?.SetCurrentGrid(value); gridHandler?.SetCurrentGrid(value); } }

    private void Awake()
    {
        if (selectedItemController == null)
            selectedItemController = FindFirstObjectByType<SelectedItemController>();
        if (gridHandler == null)
            gridHandler = FindFirstObjectByType<InventoryGridHandler>();
        if (defeatHandler == null)
            defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();
    }

    public void HandlePrimaryClick(Vector2 mousePosition)
    {
        if (selectedItemGrid != null && gridHandler != null)
        {
            HandleGridClick(mousePosition);
            return;
        }

        if (selectedItemSlot != null)
        {
            HandleSlotClick();
            return;
        }

        if (HasItemOnCursor)
        {
            if (!IsPointerOverUI(mousePosition))
                ThrowItemOnGround();
        }
    }

    private void HandleGridClick(Vector2 mousePosition)
    {
        InventoryItem selectedItem = selectedItemController.HasItem ? selectedItemController.SelectedItem : null;
        Vector2Int tilePos = gridHandler.GetTileGridPosition(mousePosition, selectedItem);

        if (selectedItem != null)
        {
            gridHandler.PlaceItemInput(selectedItemGrid, selectedItem, tilePos);
        }
        else
        {
            InventoryItem item = selectedItemGrid.PickUpItem(tilePos);
            if (item != null)
            {
                selectedItemController.PickUp(item);
                itemHighlightController?.SetSelectedItem(item);
            }
        }
    }

    private void HandleSlotClick()
    {
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

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current) { position = screenPosition };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    public void ThrowItemOnGround()
    {
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
