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
}
