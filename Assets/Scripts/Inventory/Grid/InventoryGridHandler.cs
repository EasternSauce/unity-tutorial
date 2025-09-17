using System.Collections.Generic;
using UnityEngine;

public class InventoryGridHandler : MonoBehaviour
{
    [SerializeField] private SelectedItemController selectedItemController;
    [SerializeField] private InventoryItemHighlightController itemHighlightController;
    [SerializeField] private CharacterDefeatHandler defeatHandler;

    private ItemGrid currentGrid;

    private void Awake()
    {
        if (selectedItemController == null) selectedItemController = FindFirstObjectByType<SelectedItemController>();
        if (itemHighlightController == null) itemHighlightController = FindFirstObjectByType<InventoryItemHighlightController>();
        if (defeatHandler == null) defeatHandler = FindFirstObjectByType<CharacterDefeatHandler>();
    }

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition, InventoryItem item = null)
    {
        if (currentGrid == null) return Vector2Int.zero;

        RectTransform rectTransform = currentGrid.GetComponent<RectTransform>();
        Vector2 localMousePosition;
        Camera cam = rectTransform.GetComponentInParent<Canvas>()?.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, cam, out localMousePosition);

        Vector2 pivotOffset = new Vector2(rectTransform.rect.width * rectTransform.pivot.x,
                                          rectTransform.rect.height * rectTransform.pivot.y);
        localMousePosition += pivotOffset;

        if (item != null)
        {
            localMousePosition.x -= (item.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2f;
            localMousePosition.y += (item.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2f;
        }

        int x = Mathf.FloorToInt(localMousePosition.x / ItemGrid.TileSizeWidth);
        int y = Mathf.FloorToInt((rectTransform.rect.height - localMousePosition.y) / ItemGrid.TileSizeHeight);

        x = Mathf.Clamp(x, 0, currentGrid.Width - 1);
        y = Mathf.Clamp(y, 0, currentGrid.Height - 1);

        return new Vector2Int(x, y);
    }

    public Vector2Int GetClampedTileGridPosition(Vector2 mousePosition, InventoryItem item)
    {
        if (currentGrid == null) return Vector2Int.zero;

        Vector2Int pos = GetTileGridPosition(mousePosition, item);

        if (item != null)
        {
            pos.x = Mathf.Min(pos.x, currentGrid.Width - item.itemData.sizeWidth);
            pos.y = Mathf.Min(pos.y, currentGrid.Height - item.itemData.sizeHeight);
        }

        pos.x = Mathf.Clamp(pos.x, 0, currentGrid.Width - 1);
        pos.y = Mathf.Clamp(pos.y, 0, currentGrid.Height - 1);

        return pos;
    }

    public bool IsMouseOverGrid(Vector2 mousePosition)
    {
        if (currentGrid == null) return false;
        Vector2Int pos = GetTileGridPosition(mousePosition);
        return currentGrid.BoundaryCheck(pos.x, pos.y, 1, 1);
    }

    public void InsertItem(ItemGrid grid, InventoryItem itemToInsert)
    {
        if (grid == null || itemToInsert == null) return;

        Vector2Int? posOnGrid = grid.FindSpaceForObject(itemToInsert.itemData);
        if (posOnGrid == null) return;

        grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    public void PlaceItemInput(ItemGrid grid, InventoryItem selectedItem, Vector2Int positionOnGrid)
    {
        if (selectedItem == null || grid == null) return;
        if (!grid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight)) return;

        var overlappedItems = grid.GetOverlappingItems(positionOnGrid.x, positionOnGrid.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight);

        if (overlappedItems.Count > 1) return;

        InventoryItem overlapItem = overlappedItems.Count == 1 ? overlappedItems[0] : null;

        if (overlapItem != null) grid.ClearGridFromItem(overlapItem);
        grid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);

        selectedItemController?.ClearSelectedItem();

        if (overlapItem != null)
        {
            selectedItemController?.PickUp(overlapItem);
        }
    }

    public void HandleClick(ItemGrid grid, Vector2 mousePosition)
    {
        if (!IsMouseOverGrid(mousePosition)) return;

        InventoryItem selectedItem = selectedItemController.HasItem ? selectedItemController.SelectedItem : null;
        Vector2Int tilePos = GetClampedTileGridPosition(mousePosition, selectedItem);

        if (selectedItem != null)
        {
            PlaceItemInput(grid, selectedItem, tilePos);
        }
        else
        {
            InventoryItem item = grid.PickUpItem(tilePos);
            if (item != null)
            {
                selectedItemController.PickUp(item);
            }
        }
    }
}
