using UnityEngine;

public class ItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryHighlight inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;

    private InventoryItem selectedItem;
    private Vector2Int lastPosition;

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
        if (inventoryHighlight != null)
            inventoryHighlight.SetParent(grid);
    }

    public void SetSelectedItem(InventoryItem item)
    {
        selectedItem = item;
    }

    private void Update()
    {
        if (currentGrid == null || inventoryHighlight == null)
        {
            inventoryHighlight?.Show(false);
            return;
        }

        Vector2Int positionOnGrid = GetMouseGridPosition();


        if (!currentGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y))
        {
            inventoryHighlight.Show(false);
            return;
        }

        if (positionOnGrid != lastPosition)
        {
            lastPosition = positionOnGrid;
            UpdateHighlight();
        }
    }


    private Vector2Int GetMouseGridPosition()
    {
        Vector2 mousePos = Input.mousePosition;

        if (selectedItem != null)
        {
            mousePos.x -= (selectedItem.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2;
            mousePos.y += (selectedItem.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2;
        }

        return currentGrid.GetTileGridPosition(mousePos);
    }

    private void UpdateHighlight()
    {
        if (selectedItem == null)
        {
            HighlightExistingItem();
        }
        else
        {
            HighlightSelectedItem();
        }
    }

    private void HighlightExistingItem()
    {
        if (!currentGrid.PositionCheck(lastPosition.x, lastPosition.y))
        {
            inventoryHighlight.Show(false);
            return;
        }

        InventoryItem itemToHighlight = currentGrid.GetItem(lastPosition.x, lastPosition.y);

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

    private void HighlightSelectedItem()
    {
        bool canPlace = currentGrid.BoundaryCheck(
            lastPosition.x,
            lastPosition.y,
            selectedItem.itemData.sizeWidth,
            selectedItem.itemData.sizeHeight
        );

        inventoryHighlight.Show(canPlace);
        inventoryHighlight.SetSize(selectedItem);
        inventoryHighlight.SetPosition(currentGrid, selectedItem, lastPosition.x, lastPosition.y);
    }
}
