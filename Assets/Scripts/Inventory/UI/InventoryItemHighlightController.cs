using UnityEngine;

public class InventoryItemHighlightController : MonoBehaviour
{
    [SerializeField] private InventoryHighlighter inventoryHighlight;
    [SerializeField] private ItemGrid currentGrid;
    [SerializeField] private InventoryGridHandler gridHandler;
    [SerializeField] private ItemTooltip tooltip;

    private InventoryItem selectedItem;
    private InventoryItem lastHighlightedItem;
    private Canvas parentCanvas;

    private void Awake()
    {
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
        UpdateHighlightUnderCursor();
    }

    public void SetSelectedItem(InventoryItem item)
    {
        selectedItem = item;
    }

    public void ClearHighlight()
    {
        if (inventoryHighlight != null) inventoryHighlight.Show(false);
        lastHighlightedItem = null;
        tooltip?.Hide();
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
        UpdateHighlightUnderCursor();
    }

    private bool IsPointerInsideGrid()
    {
        RectTransform rectTransform = currentGrid.GetComponent<RectTransform>();
        Vector2 localMousePos;
        Camera cam = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? parentCanvas.worldCamera : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, cam, out localMousePos);
        return rectTransform.rect.Contains(localMousePos);
    }

    private void UpdateHighlightUnderCursor()
    {
        Vector2Int tilePos = gridHandler.GetClampedTileGridPosition(Input.mousePosition, selectedItem);
        InventoryItem itemUnderCursor = null;

        if (selectedItem == null)
        {
            var overlappingItems = currentGrid.GetOverlappingItems(tilePos.x, tilePos.y, 1, 1);
            Vector2 mousePos = Input.mousePosition;
            foreach (var item in overlappingItems)
            {
                RectTransform rt = item.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos))
                {
                    itemUnderCursor = item;
                    break;
                }
            }
        }
        else
        {
            bool canPlace = currentGrid.BoundaryCheck(tilePos.x, tilePos.y, selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight);
            if (canPlace)
                itemUnderCursor = selectedItem;
        }

        if (itemUnderCursor != lastHighlightedItem)
        {
            if (itemUnderCursor != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemUnderCursor);
                if (selectedItem == null)
                    inventoryHighlight.SetPosition(currentGrid, itemUnderCursor);
                else
                    inventoryHighlight.SetPosition(currentGrid, selectedItem, tilePos.x, tilePos.y);
                inventoryHighlight.transform.SetAsFirstSibling();
                if (selectedItem == null)
                {
                    string text = ItemTooltipBuilder.BuildTooltip(itemUnderCursor.itemData);
                    tooltip?.Show(text, itemUnderCursor.itemData.icon, Input.mousePosition);
                }
            }
            else
            {
                inventoryHighlight.Show(false);
                tooltip?.Hide();
            }
            lastHighlightedItem = itemUnderCursor;
        }
    }
}
