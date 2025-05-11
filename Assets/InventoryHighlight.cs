using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;


    public void SetSize(InventoryItem inventoryItem)
    {
        Vector2 size = new Vector2();
        size.x = inventoryItem.itemData.sizeWidth * ItemGrid.TileSizeWidth;
        size.y = inventoryItem.itemData.sizeHeight * ItemGrid.TileSizeHeight;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        highlighter.SetParent(targetGrid.transform);

        Vector2 position = targetGrid.CalculatePositionOfObjectOnGrid(targetItem,
            targetItem.positionOnGridX,
            targetItem.positionOnGridY
            );
        
        highlighter.localPosition = position;
    }

    public void Show(bool set)
    {
        highlighter.gameObject.SetActive(set);
    }
}
