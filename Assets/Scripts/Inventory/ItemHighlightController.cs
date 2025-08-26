using UnityEngine;

public class ItemHighlightController
{
    private InventoryHighlight inventoryHighlight;
    private ItemGrid currentGrid;
    private InventoryItem itemToHighlight;
    private Vector2Int lastPosition;

    public ItemHighlightController(InventoryHighlight highlight)
    {
        inventoryHighlight = highlight;
    }

    public void SetParent(ItemGrid grid)
    {
        currentGrid = grid;
        inventoryHighlight.SetParent(grid);
    }

    public void UpdateHighlight(InventoryItem selectedItem, ItemGrid selectedGrid, Vector2Int positionOnGrid)
    {
        if (selectedGrid == null || currentGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        if (positionOnGrid == lastPosition) return;

        if (!currentGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y)) return;

        lastPosition = positionOnGrid;

        if (selectedItem == null)
        {
            HighlightExistingItem(positionOnGrid);
        }
        else
        {
            HighlightSelectedItem(selectedItem, positionOnGrid);
        }
    }

    private void HighlightExistingItem(Vector2Int positionOnGrid)
    {
        itemToHighlight = currentGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

        if (itemToHighlight != null)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(itemToHighlight);
            inventoryHighlight.SetPosition(currentGrid, itemToHighlight);
        }
        else
        {
            inventoryHighlight.Show(false);
        }
    }

    private void HighlightSelectedItem(InventoryItem selectedItem, Vector2Int positionOnGrid)
    {
        bool canPlace = currentGrid.BoundaryCheck(
            positionOnGrid.x,
            positionOnGrid.y,
            selectedItem.itemData.sizeWidth,
            selectedItem.itemData.sizeHeight
        );

        inventoryHighlight.Show(canPlace);
        inventoryHighlight.SetSize(selectedItem);
        inventoryHighlight.SetPosition(currentGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
    }
}
