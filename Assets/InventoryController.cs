using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    Vector2Int positionOnGrid;
    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform selectedItemRectTransform;

    [SerializeField] List<ItemData> itemDatas;
    [SerializeField] GameObject inventoryItemPrefab;
    [SerializeField] Transform targetCanvas;

    [SerializeField] InventoryHighlight inventoryHighlight;
    [SerializeField] RectTransform selectedItemParent;

    InventoryItem itemToHighlight;

    public EquipmentItemSlot SelectedItemSlot
    {
        get => selectedItemSlot;
        set
        {
            selectedItemSlot = value;
        }
    }

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    private void Update()
    {
        ProcessMouseInput();

        HandleHighlight();

        if (selectedItemGrid == null) { return; }

        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            InsertRandomItem();
        }
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = SelectedItemGrid.FindSpaceForObject(itemToInsert.itemData);

        if (posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;

    private void HandleHighlight()
    {
        if (selectedItemSlot != null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int positionOnGrid = GetTileGridPosition();
        if (positionOnGrid == oldPosition) { return; }

        if (selectedItemGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y) == false) { return; }

        oldPosition = positionOnGrid;

        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundaryCheck(
                positionOnGrid.x,
                positionOnGrid.y,
                selectedItem.itemData.sizeWidth,
                selectedItem.itemData.sizeHeight
            ));

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);

        }
    }

    private void CreateRandomItem()
    {
        if (selectedItem != null) { return; }
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

    }

    private void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = Input.mousePosition;
        }

        if (selectedItemGrid == null && selectedItemSlot == null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            if (selectedItemGrid != null)
            {
                ItemGridInput();
            }

            if (selectedItemSlot != null)
            {
                ItemSlotInput();
            }
        }
    }

    private void ItemSlotInput()
    {
        if (selectedItem != null)
        {
            PlaceItemIntoSlot();
        }
    }

    private void PlaceItemIntoSlot()
    {
        if (selectedItemSlot.Check(selectedItem) == false) { return; }

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
    }

    private void ItemGridInput()
    {
        positionOnGrid = GetTileGridPosition();
        if (selectedItem == null)
        {
            InventoryItem itemToSelect = selectedItemGrid.PickUpItem(positionOnGrid);
            if (itemToSelect != null)
            {
                SelectItem(itemToSelect);
            }
        }
        else
        {
            PlaceItemInput();
        }
    }

    Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2;
            position.y += (selectedItem.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItemInput()
    {
        if (selectedItemGrid.BoundaryCheck(
            positionOnGrid.x,
            positionOnGrid.y,
            selectedItem.itemData.sizeWidth,
            selectedItem.itemData.sizeHeight) == false
            )
        {
            return;
        }

        if (selectedItemGrid.CheckOverlap(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight, ref overlapItem) == false)
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
            overlapItem = null;
        }

    }
}
