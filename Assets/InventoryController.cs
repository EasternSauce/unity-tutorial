using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] private ItemGrid selectedItemGrid;

    Vector2Int positionOnGrid;
    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform selectedItemRectTransform;

    [SerializeField] List<ItemData> itemDatas;
    [SerializeField] GameObject inventoryItemPrefab;
    [SerializeField] Transform targetCanvas;

    [SerializeField] InventoryHighlight inventoryHighlight;

    InventoryItem itemToHighlight;

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

        if (selectedItemGrid == null) { return; }

        HandleHighlight();

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
        Vector2Int? posOnGrid = SelectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) { return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (positionOnGrid == oldPosition) { return; }

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
        GameObject newItemGO = Instantiate(inventoryItemPrefab, targetCanvas);

        InventoryItem newInventoryItem = newItemGO.GetComponent<InventoryItem>();
        selectedItem = newInventoryItem;

        RectTransform newItemRectTransform = newItemGO.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);
        selectedItemRectTransform = newItemRectTransform;

        int selectedItemId = UnityEngine.Random.Range(0, itemDatas.Count);
        newInventoryItem.Set(itemDatas[selectedItemId]);
    }

    private void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = Input.mousePosition;
        }

        if (selectedItemGrid == null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            positionOnGrid = GetTileGridPosition();
            if (selectedItem == null)
            {
                selectedItem = selectedItemGrid.PickUpItem(positionOnGrid);
                if (selectedItem != null)
                {
                    selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
                }
            }
            else
            {
                PlaceItemInput();
            }
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
        selectedItem = null;
        selectedItemRectTransform = null;

        if (overlapItem != null)
        {
            selectedItem = overlapItem;
            selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
            overlapItem = null;
        }

    }
}
