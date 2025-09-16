using UnityEngine;

public class InventoryItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController; // add this

    [SerializeField] private InventoryHighlighter inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private ItemTooltip tooltip;

    private InventoryItem selectedItem;
    private Vector2Int lastPosition = new Vector2Int(int.MinValue, int.MinValue);
    private Canvas parentCanvas;

    private void Awake()
    {
        if (inventoryController == null)
            inventoryController = FindFirstObjectByType<InventoryController>(); // fallback

        if (gridHandler == null)
            gridHandler = FindFirstObjectByType<InventoryGridHandler>();
        if (tooltip == null)
            Debug.LogWarning($"Tooltip not assigned on {name}", this);
    }

    public void SetCurrentGrid(ItemGrid grid)
    {
        currentGrid = grid;
        if (inventoryHighlight != null)
            inventoryHighlight.SetParent(grid);
        parentCanvas = grid != null ? grid.GetComponentInParent<Canvas>() : null;

        if (currentGrid == null || inventoryHighlight == null) return;
        if (!currentGrid.gameObject.activeInHierarchy || !IsPointerInsideGrid()) return;

        Vector2Int pos = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
        lastPosition = pos;
        UpdateHighlight(pos);
    }

    public void SetSelectedItem(InventoryItem item) => selectedItem = item;

    public void ClearHighlight()
    {
        if (inventoryHighlight != null)
            inventoryHighlight.Show(false);
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

            if (inventoryController.SelectedItemSlot == null)
                tooltip?.Hide();

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
        if (selectedItem == null)
        {
            HighlightExistingItem(positionOnGrid);
            UpdateTooltipForItem(positionOnGrid);
        }
        else
        {
            HighlightSelectedItem(positionOnGrid);
        }
    }

    private void UpdateTooltipForItem(Vector2Int position)
    {
        if (!currentGrid.PositionCheck(position.x, position.y))
        {
            tooltip?.Hide();
            return;
        }

        InventoryItem item = currentGrid.GetItem(position.x, position.y);
        if (item != null)
        {
            if (tooltip.CurrentTarget != item.gameObject)
                tooltip?.Show(ItemTooltipBuilder.BuildTooltip(item.itemData), item.itemData.icon, true, item.gameObject);
        }
        else
        {
            if (tooltip.CurrentTarget != null)
                tooltip.Hide();
        }
    }

    private void HighlightExistingItem(Vector2Int position)
    {
        if (!currentGrid.PositionCheck(position.x, position.y))
        {
            tooltip?.Hide();
            return;
        }

        InventoryItem item = currentGrid.GetItem(position.x, position.y);
        if (item != null)
        {
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(item);
            inventoryHighlight.SetPosition(currentGrid, item);
            inventoryHighlight.transform.SetAsFirstSibling();

            if (tooltip.CurrentTarget != item.gameObject)
                tooltip?.ShowForItem(item, true, item.gameObject);
        }
        else
        {
            inventoryHighlight.Show(false);
            if (tooltip.CurrentTarget != null)
                tooltip.Hide();
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
    }
}
