using UnityEngine;

public class InventoryItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private InventoryHighlighter inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private TooltipController tooltipController;

    private InventoryItem selectedItem;
    private Vector2Int lastPosition = new Vector2Int(int.MinValue, int.MinValue);
    private InventoryItem currentHoverItem;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (inventoryController == null)
            inventoryController = FindFirstObjectByType<InventoryController>();

        if (gridHandler == null)
            gridHandler = FindFirstObjectByType<InventoryGridHandler>();

        if (tooltipController == null)
            tooltipController = FindFirstObjectByType<TooltipController>();

        if (inventoryHighlight == null)
            Debug.LogWarning($"InventoryHighlight not assigned on {name}", this);
    }

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
        if (inventoryHighlight != null)
            inventoryHighlight.SetParent(grid);

        parentCanvas = grid != null ? grid.GetComponentInParent<Canvas>() : null;

        if (currentGrid == null || inventoryHighlight == null) return;

        Vector2Int pos = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
        lastPosition = pos;
        UpdateHighlight(pos);
    }

    public void SetSelectedItem(InventoryItem item) => selectedItem = item;

    private void Update()
    {
        if (currentGrid == null || inventoryHighlight == null || gridHandler == null)
        {
            ClearHover();
            return;
        }

        if (!currentGrid.gameObject.activeInHierarchy || !IsPointerInsideGrid())
        {
            ClearHover();
            return;
        }

        Vector2Int positionOnGrid = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
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
        Camera cam = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? parentCanvas.worldCamera : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out localMousePos);
        return rectTransform.rect.Contains(localMousePos);
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
            ClearHover();
            inventoryHighlight.Show(false);
            return;
        }

        InventoryItem item = currentGrid.GetItem(position.x, position.y);
        if (item != currentHoverItem)
        {
            if (currentHoverItem != null)
                tooltipController?.ClearHoverTooltip(currentHoverItem.gameObject);

            currentHoverItem = item;

            if (currentHoverItem != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(currentHoverItem);
                inventoryHighlight.SetPosition(currentGrid, currentHoverItem);
                inventoryHighlight.transform.SetAsFirstSibling();
                tooltipController?.ShowHoverTooltip(currentHoverItem);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
    }

    private void HighlightSelectedItem(Vector2Int position)
    {
        if (selectedItem == null) return;

        bool canPlace = currentGrid.BoundaryCheck(position.x, position.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight);
        inventoryHighlight.Show(canPlace);

        if (canPlace)
        {
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(currentGrid, selectedItem, position.x, position.y);
            inventoryHighlight.transform.SetAsFirstSibling();
        }

        if (currentHoverItem != null)
        {
            tooltipController?.ClearHoverTooltip(currentHoverItem.gameObject);
            currentHoverItem = null;
        }
    }

    public void ClearHover()
    {
        if (currentHoverItem != null)
        {
            tooltipController?.ClearHoverTooltip(currentHoverItem.gameObject);
            currentHoverItem = null;
        }

        inventoryHighlight.Show(false);
        lastPosition = new Vector2Int(int.MinValue, int.MinValue);
    }
}
