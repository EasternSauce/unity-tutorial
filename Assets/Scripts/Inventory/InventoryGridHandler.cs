using UnityEngine;

public class InventoryGridHandler : MonoBehaviour
{
    private ItemGrid currentGrid;

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition, InventoryItem item = null)
    {
        if (currentGrid == null)
        {
            Debug.LogWarning("InventoryGridHandler: No grid set for GetTileGridPosition.");
            return Vector2Int.zero;
        }

        RectTransform rectTransform = currentGrid.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            null,
            out Vector2 localMousePosition
        );

        if (item != null)
        {
            localMousePosition.x -= (item.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2;
            localMousePosition.y += (item.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2;
        }

        int x = Mathf.FloorToInt(localMousePosition.x / ItemGrid.TileSizeWidth);
        int y = Mathf.FloorToInt(-localMousePosition.y / ItemGrid.TileSizeHeight);

        return new Vector2Int(x, y);
    }

    public void InsertItem(ItemGrid grid, InventoryItem itemToInsert)
    {
        if (grid == null) return;

        Vector2Int? posOnGrid = grid.FindSpaceForObject(itemToInsert.itemData);
        if (posOnGrid == null) return;

        grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    public void PlaceItemInput(ItemGrid grid, InventoryItem selectedItem, SelectedItemController selectedItemController, ItemHighlightController itemHighlightController, Vector2Int positionOnGrid)
    {
        if (selectedItem == null || grid == null) return;

        if (!grid.BoundaryCheck(
                positionOnGrid.x, positionOnGrid.y,
                selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight))
            return;

        var overlappedItems = grid.GetOverlappingItems(
            positionOnGrid.x, positionOnGrid.y,
            selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight
        );

        if (overlappedItems.Count > 1) return;

        InventoryItem overlapItem = overlappedItems.Count == 1 ? overlappedItems[0] : null;

        if (overlapItem != null)
            grid.ClearGridFromItem(overlapItem);

        grid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);

        selectedItemController.ClearSelectedItem();
        itemHighlightController.SetSelectedItem(null);

        if (overlapItem != null)
        {
            selectedItemController.SetSelectedItem(overlapItem);
            itemHighlightController.SetSelectedItem(overlapItem);
        }
    }

    public void ItemGridInput(Vector2 mousePosition, SelectedItemController selectedItemController, ItemHighlightController itemHighlightController, CharacterDefeatHandler defeatHandler)
    {
        if (currentGrid == null || defeatHandler != null && defeatHandler.IsDefeated)
            return;

        Vector2Int positionOnGrid = GetTileGridPosition(mousePosition,
            selectedItemController.HasItem ? selectedItemController.SelectedItem : null);

        if (!currentGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y))
            return;

        if (!selectedItemController.HasItem)
        {
            InventoryItem itemToSelect = currentGrid.PickUpItem(positionOnGrid);
            if (itemToSelect != null)
            {
                selectedItemController.PickUp(itemToSelect);
                itemHighlightController.SetSelectedItem(itemToSelect);
            }
        }
        else
        {
            PlaceItemInput(currentGrid, selectedItemController.SelectedItem, selectedItemController, itemHighlightController, positionOnGrid);
        }
    }
}