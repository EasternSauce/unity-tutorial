using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int positionOnGridX;
    public int positionOnGridY;

    public void Set(ItemData itemData)
    {
        this.itemData = itemData;
        GetComponent<Image>().sprite = itemData.icon;
        Vector2 size = new Vector2(
            itemData.sizeWidth * ItemGrid.TileSizeWidth,
            itemData.sizeHeight * ItemGrid.TileSizeHeight
            );
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
