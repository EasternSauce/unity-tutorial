using UnityEngine;

public class ItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryHighlight inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;

    private InventoryItem selectedItem;
    private Vector2Int lastPosition = new Vector2Int(-1, -1);

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

        if (!IsPointerInsideGrid())
        {
            inventoryHighlight.Show(false);
            lastPosition = new Vector2Int(-1, -1);
            return;
        }

        Vector2Int positionOnGrid = currentGrid.GetTileGridPosition(Input.mousePosition, selectedItem);

        if (positionOnGrid != lastPosition)
        {
            lastPosition = positionOnGrid;
            UpdateHighlight(positionOnGrid);
        }
    }

    private bool IsPointerInsideGrid()
    {
        RectTransform rectTransform = currentGrid.GetComponent<RectTransform>();
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            null,
            out localMousePos
        );

        Rect rect = rectTransform.rect;
        return rect.Contains(localMousePos);
    }

    private void UpdateHighlight(Vector2Int positionOnGrid)
    {
        if (selectedItem == null)
        {
            HighlightExistingItem(positionOnGrid);
        }
        else
        {
            HighlightSelectedItem(positionOnGrid);
        }
    }

    private void HighlightExistingItem(Vector2Int position)
    {
        if (!currentGrid.PositionCheck(position.x, position.y))
        {
            inventoryHighlight.Show(false);
            return;
        }

        InventoryItem item = currentGrid.GetItem(position.x, position.y);
        if (item != null)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(item);
            inventoryHighlight.SetPosition(currentGrid, item);
        }
        else
        {
            inventoryHighlight.Show(false);
        }
    }

    private void HighlightSelectedItem(Vector2Int position)
    {
        bool canPlace = currentGrid.BoundaryCheck(
            position.x,
            position.y,
            selectedItem.itemData.sizeWidth,
            selectedItem.itemData.sizeHeight
        );

        if (canPlace)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(currentGrid, selectedItem, position.x, position.y);
        }
        else
        {
            inventoryHighlight.Show(false);
        }
    }
}
