using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public bool HasItemOnCursor => selectedItemController.HasItem;

    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
    [SerializeField] private ItemHighlightController itemHighlightController;
    [SerializeField] private RectTransform selectedItemParent;
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private CharacterDefeatHandler defeatHandler;

    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;
    private Vector2 mousePosition;
    private Vector2Int positionOnGrid;

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
            itemHighlightController.SetCurrentGrid(value);

            if (gridHandler != null)
            {
                gridHandler.SetCurrentGrid(value);
            }
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
    }

    private void Update()
    {
        mousePosition = mouseInput.mouseInputPosition;
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItemController.SelectedItem;
        selectedItemController.ClearSelectedItem();
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = SelectedItemGrid.FindSpaceForObject(itemToInsert.itemData);
        if (posOnGrid == null) return;

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void CreateRandomItem()
    {
        if (selectedItemController.HasItem) return;

        int selectedItemId = UnityEngine.Random.Range(0, itemDatas.Count);
        InventoryItem newItem = CreateNewInventoryItem(itemDatas[selectedItemId]);
        selectedItemController.SetSelectedItem(newItem);
        itemHighlightController.SetSelectedItem(newItem);
    }

    public InventoryItem CreateNewInventoryItem(ItemData itemData)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, targetCanvas);
        InventoryItem newInventoryItem = newItemGO.GetComponent<InventoryItem>();
        RectTransform newItemRectTransform = newItemGO.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);
        newInventoryItem.Set(itemData);
        return newInventoryItem;
    }

    public void ProcessLMBPress(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (defeatHandler == null)
            defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();

        if (defeatHandler != null && defeatHandler.IsDefeated)
            return;

        mousePosition = mouseInput.mouseInputPosition;
        bool hasItem = selectedItemController.HasItem;

        bool overUI = IsPointerOverUI(mousePosition);

        if (SelectedItemGrid != null)
        {
            ItemGridInput();
            return;
        }

        if (selectedItemSlot != null)
        {
            ItemSlotInput();
            return;
        }

        if (hasItem && !overUI)
        {
            ThrowItemOnGround();
        }
    }


    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void ThrowItemOnGround()
    {
        InventoryItem itemToDrop = selectedItemController.Drop();
        if (itemToDrop != null)
        {
            DropItem(GameManager.instance.playerObject.transform.position, itemToDrop);
        }
    }

    public void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return;

        // Use ItemSpawnManager to spawn the item in the current area's scene
        ItemSpawnManager.instance.SpawnItem(dropPosition, itemToDrop.itemData);

        // Destroy the inventory object
        Destroy(itemToDrop.gameObject);
    }

    private void DestroyInventoryObject(InventoryItem itemToDrop)
    {
        Destroy(itemToDrop.gameObject);
    }

    private void ItemSlotInput()
    {
        if (!selectedItemController.HasItem)
        {
            InventoryItem item = selectedItemSlot.PickUpItem();
            if (item != null)
            {
                selectedItemController.PickUp(item);
                itemHighlightController.SetSelectedItem(item);
            }
        }
        else
        {
            PlaceItemIntoSlot();
        }
    }

    private void PlaceItemIntoSlot()
    {
        if (!selectedItemSlot.Check(selectedItemController.SelectedItem)) return;

        InventoryItem replacedItem = selectedItemSlot.ReplaceItem(selectedItemController.SelectedItem);
        if (replacedItem == null)
        {
            selectedItemController.ClearSelectedItem();
            itemHighlightController.SetSelectedItem(null);
        }
        else
        {
            selectedItemController.SetSelectedItem(replacedItem);
            itemHighlightController.SetSelectedItem(replacedItem);
        }
    }

    private void ItemGridInput()
    {
        if (selectedItemGrid == null || defeatHandler != null && defeatHandler.IsDefeated)
            return;

        if (gridHandler == null)
        {
            Debug.LogWarning("InventoryController: InventoryGridHandler is not assigned.");
            return;
        }

        positionOnGrid = gridHandler.GetTileGridPosition(mousePosition,
            selectedItemController.HasItem ? selectedItemController.SelectedItem : null);

        if (!selectedItemGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y))
            return;

        if (!selectedItemController.HasItem)
        {
            InventoryItem itemToSelect = selectedItemGrid.PickUpItem(positionOnGrid);
            if (itemToSelect != null)
            {
                selectedItemController.PickUp(itemToSelect);
                itemHighlightController.SetSelectedItem(itemToSelect);
            }
        }
        else
        {
            PlaceItemInput();
        }
    }

    private void PlaceItemInput()
    {
        if (!selectedItemController.HasItem) return;

        InventoryItem selectedItem = selectedItemController.SelectedItem;

        if (!selectedItemGrid.BoundaryCheck(
                positionOnGrid.x, positionOnGrid.y,
                selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight))
            return;

        var overlappedItems = selectedItemGrid.GetOverlappingItems(
            positionOnGrid.x, positionOnGrid.y,
            selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight
        );

        if (overlappedItems.Count > 1) return;

        InventoryItem overlapItem = overlappedItems.Count == 1 ? overlappedItems[0] : null;

        if (overlapItem != null)
            selectedItemGrid.ClearGridFromItem(overlapItem);

        selectedItemGrid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);

        selectedItemController.ClearSelectedItem();
        itemHighlightController.SetSelectedItem(null);

        if (overlapItem != null)
        {
            selectedItemController.SetSelectedItem(overlapItem);
            itemHighlightController.SetSelectedItem(overlapItem);
        }
    }
}
