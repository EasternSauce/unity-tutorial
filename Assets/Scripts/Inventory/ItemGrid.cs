using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    InventoryItem[,] inventoryItemGrid;

    public const float TileSizeWidth = 32f;
    public const float TileSizeHeight = 32f;

    [SerializeField] int gridSizeWidth;
    [SerializeField] int gridSizeHeight;

    RectTransform rectTransform;

    Vector2 mousePositionOnTheGrid;
    Vector2Int tileGridPosition = new Vector2Int();

    [SerializeField] GameObject inventoryItemPrefab;

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        inventoryItemGrid = new InventoryItem[gridSizeWidth, gridSizeHeight];
        Vector2 size = new()
        {
            x = TileSizeWidth * gridSizeWidth,
            y = TileSizeHeight * gridSizeHeight
        };
        rectTransform.sizeDelta = size;
    }

    public void PlaceItem(InventoryItem itemToPlace, int x, int y)
    {
        RectTransform itemRectTransform = itemToPlace.GetComponent<RectTransform>();
        itemRectTransform.SetParent(transform);

        for (int ix = 0; ix < itemToPlace.itemData.sizeWidth; ix++)
        {
            for (int iy = 0; iy < itemToPlace.itemData.sizeHeight; iy++)
            {
                inventoryItemGrid[x + ix, y + iy] = itemToPlace;
            }
        }

        itemToPlace.positionOnGridX = x;
        itemToPlace.positionOnGridY = y;

        itemRectTransform.localPosition = CalculatePositionOfObjectOnGrid(itemToPlace, x, y);
    }

    public Vector2Int? FindSpaceForObject(ItemData itemData)
    {
        int width = gridSizeWidth - itemData.sizeWidth + 1;
        int height = gridSizeHeight - itemData.sizeHeight + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemData.sizeWidth, itemData.sizeHeight) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    private bool CheckAvailableSpace(int posX, int posY, int sizeWidth, int sizeHeight)
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public InventoryItem GetItem(int x, int y)
    {
        return inventoryItemGrid[x, y];
    }

    public Vector2 CalculatePositionOfObjectOnGrid(InventoryItem item, int x, int y)
    {
        Vector2 positionOnGrid = new()
        {
            x = TileSizeWidth * x + TileSizeWidth * item.itemData.sizeWidth / 2,
            y = -(TileSizeHeight * y + TileSizeHeight * item.itemData.sizeHeight / 2)
        };
        return positionOnGrid;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            null,
            out Vector2 localMousePosition
        );

        tileGridPosition.x = (int)(localMousePosition.x / TileSizeWidth);
        tileGridPosition.y = (int)(-localMousePosition.y / TileSizeHeight);
        return tileGridPosition;
    }

    public InventoryItem PickUpItem(Vector2Int tilePositionOnGrid)
    {
        InventoryItem pickedItem = inventoryItemGrid[tilePositionOnGrid.x, tilePositionOnGrid.y];

        if (pickedItem == null) { return null; }

        ClearGridFromItem(pickedItem);

        return pickedItem;
    }

    public void ClearGridFromItem(InventoryItem pickedItem)
    {
        for (int ix = 0; ix < pickedItem.itemData.sizeWidth; ix++)
        {
            for (int iy = 0; iy < pickedItem.itemData.sizeHeight; iy++)
            {
                inventoryItemGrid[pickedItem.positionOnGridX + ix, pickedItem.positionOnGridY + iy] = null;
            }
        }
    }

    public bool PositionCheck(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x >= gridSizeWidth || y >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false) { return false; }

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false) { return false; }

        return true;
    }

    public bool CheckOverlap(int posX, int posY, int sizeWidth, int sizeHeight, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemGrid[posX + x, posY + y];
                    }
                    else
                    {
                        if (overlapItem != inventoryItemGrid[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

}
