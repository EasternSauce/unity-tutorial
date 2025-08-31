using UnityEngine;

public class ItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryHighlight inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;
    [SerializeField] private InventoryGridHandler gridHandler;

    private InventoryItem selectedItem;
    private Vector2Int lastPosition = new Vector2Int(int.MinValue, int.MinValue);
    private Canvas parentCanvas;

    private void Awake()
    {
        if (gridHandler == null)
            gridHandler = FindFirstObjectByType<InventoryGridHandler>();
    }

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
        if (inventoryHighlight != null)
            inventoryHighlight.SetParent(grid);

        parentCanvas = grid != null ? grid.GetComponentInParent<Canvas>() : null;

        if (currentGrid == null || inventoryHighlight == null)
        {
            ClearHighlight();
            return;
        }

        if (!currentGrid.gameObject.activeInHierarchy || !IsPointerInsideGrid())
        {
            ClearHighlight();
            return;
        }

        Vector2Int pos = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
        lastPosition = pos;
        UpdateHighlight(pos);
    }

    public void SetSelectedItem(InventoryItem item)
    {
        selectedItem = item;
    }

    public void ClearHighlight()
    {
        if (inventoryHighlight != null) inventoryHighlight.Show(false);
        lastPosition = new Vector2Int(int.MinValue, int.MinValue);
    }

    private void Update()
    {
        if (currentGrid == null || inventoryHighlight == null || gridHandler == null)
        {
            ClearHighlight();
            return;
        }

        if (!currentGrid.gameObject.activeInHierarchy || !IsPointerInsideGrid())
        {
            ClearHighlight();
            return;
        }

        Vector2Int positionOnGrid = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
        if (positionOnGrid == lastPosition) return;
        lastPosition = positionOnGrid;
        UpdateHighlight(positionOnGrid);
    }

    private bool IsPointerInsideGrid()
    {
        RectTransform rectTransform = currentGrid.GetComponent<RectTransform>();
        Vector2 localMousePos;
        Camera cam = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? parentCanvas.worldCamera : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out localMousePos);
        return rectTransform.rect.Contains(localMousePos);
    }

    private void UpdateHighlight(Vector2Int positionOnGrid)
    {
        if (selectedItem == null) HighlightExistingItem(positionOnGrid);
        else HighlightSelectedItem(positionOnGrid);
    }

    private void HighlightExistingItem(Vector2Int position)
    {
        if (!currentGrid.PositionCheck(position.x, position.y))
        {
            ClearHighlight();
            return;
        }
        InventoryItem item = currentGrid.GetItem(position.x, position.y);
        if (item != null)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(item);
            inventoryHighlight.SetPosition(currentGrid, item);
            inventoryHighlight.transform.SetAsFirstSibling();
        }
        else ClearHighlight();
    }

    private void HighlightSelectedItem(Vector2Int position)
    {
        if (selectedItem == null)
        {
            ClearHighlight();
            return;
        }
        bool canPlace = currentGrid.BoundaryCheck(position.x, position.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight);
        if (canPlace)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(currentGrid, selectedItem, position.x, position.y);
            inventoryHighlight.transform.SetAsFirstSibling();
        }
        else ClearHighlight();
    }
}
