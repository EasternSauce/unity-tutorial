using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public bool HasItemOnCursor => selectedItem != null;

    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    [SerializeField] MouseInput mouseInput;
    Vector2 mousePosition;
    Vector2Int positionOnGrid;
    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform selectedItemRectTransform;

    [SerializeField] List<ItemData> itemDatas;
    [SerializeField] GameObject inventoryItemPrefab;
    [SerializeField] Transform targetCanvas;

    [SerializeField] ItemHighlightController itemHighlightController;
    [SerializeField] RectTransform selectedItemParent;

    private bool isOverUIElement;

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
        }
    }

    private void Update()
    {
        isOverUIElement = EventSystem.current.IsPointerOverGameObject();

        ProcessMousePosition();
        ProcessMouseInput();
    }

    private void ProcessMousePosition()
    {
        mousePosition = mouseInput.mouseInputPosition;
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
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
        if (selectedItem != null) return;

        int selectedItemId = UnityEngine.Random.Range(0, itemDatas.Count);
        InventoryItem newItem = CreateNewInventoryItem(itemDatas[selectedItemId]);
        SelectItem(newItem);
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

    public void SelectItem(InventoryItem inventoryItem)
    {
        selectedItem = inventoryItem;
        selectedItemRectTransform = inventoryItem.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(selectedItemParent);

        // update highlight controller with new selected item
        itemHighlightController.SetSelectedItem(selectedItem);
    }

    public void ProcessLMBPress(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (selectedItemGrid == null && selectedItemSlot == null)
        {
            if (isOverUIElement) return;
            ThrowItemOnGround();
        }

        if (SelectedItemGrid != null) ItemGridInput();
        if (selectedItemSlot != null) ItemSlotInput();
    }

    public void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = mousePosition;
        }
    }

    public void ThrowItemOnGround()
    {
        DropItem(GameManager.instance.playerObject.transform.position, selectedItem);
    }

    public void DropItem(Vector3 dropPosition, InventoryItem itemToDrop)
    {
        if (itemToDrop == null) return;

        ItemSpawnManager.instance.SpawnItem(dropPosition, itemToDrop.itemData);
        DestroyInventoryObject(itemToDrop);
    }

    private void DestroyInventoryObject(InventoryItem itemToDrop)
    {
        Destroy(itemToDrop.gameObject);
        if (itemToDrop == selectedItem) NullSelectedItem();
    }

    private void ItemSlotInput()
    {
        if (selectedItem != null)
        {
            PlaceItemIntoSlot();
        }
        else
        {
            PickUpItemFromSlot();
        }
    }

    private void PickUpItemFromSlot()
    {
        InventoryItem item = selectedItemSlot.PickUpItem();
        if (item != null) SelectItem(item);
    }

    private void PlaceItemIntoSlot()
    {
        if (!selectedItemSlot.Check(selectedItem)) return;

        InventoryItem replacedItem = selectedItemSlot.ReplaceItem(selectedItem);
        if (replacedItem == null)
        {
            NullSelectedItem();
        }
        else
        {
            SelectItem(replacedItem);
        }
    }

    private void NullSelectedItem()
    {
        selectedItem = null;
        selectedItemRectTransform = null;

        // update highlight controller
        itemHighlightController.SetSelectedItem(null);
    }

    private void ItemGridInput()
    {
        positionOnGrid = GetTileGridPosition();
        if (selectedItem == null)
        {
            InventoryItem itemToSelect = selectedItemGrid.PickUpItem(positionOnGrid);
            if (itemToSelect != null) SelectItem(itemToSelect);
        }
        else
        {
            PlaceItemInput();
        }
    }

    Vector2Int GetTileGridPosition()
    {
        Vector2 position = mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2;
            position.y += (selectedItem.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItemInput()
    {
        if (!selectedItemGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight))
            return;

        if (!selectedItemGrid.CheckOverlap(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight, ref overlapItem))
        {
            overlapItem = null;
            return;
        }

        if (overlapItem != null)
        {
            selectedItemGrid.ClearGridFromItem(overlapItem);
        }

        selectedItemGrid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);
        NullSelectedItem();

        if (overlapItem != null)
        {
            selectedItem = overlapItem;
            selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
            itemHighlightController.SetSelectedItem(selectedItem);
            overlapItem = null;
        }
    }
}
