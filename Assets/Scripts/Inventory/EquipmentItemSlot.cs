using System;
using UnityEngine;

public class EquipmentItemSlot : MonoBehaviour
{
    [SerializeField] EquipmentSlot equipmentSlot;

    InventoryItem itemInSlot;

    RectTransform slotRectTransform;

    Inventory inventory;

    private void Awake()
    {
        slotRectTransform = GetComponent<RectTransform>();
    }

    public void Init(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public bool Check(InventoryItem itemToPlace)
    {
        return equipmentSlot == itemToPlace.itemData.equipmentSlot;
    }

    public InventoryItem ReplaceItem(InventoryItem itemToPlace)
    {
        InventoryItem replaceItem = PickUpItem();

        PlaceItem(itemToPlace);

        return replaceItem;
    }

    public void PlaceItem(InventoryItem itemToPlace)
    {
        itemInSlot = itemToPlace;
        inventory.AddStats(itemInSlot.itemData.stats);

        RectTransform rt = itemToPlace.GetComponent<RectTransform>();
        rt.SetParent(slotRectTransform);
        rt.position = slotRectTransform.position;
    }

    public InventoryItem PickUpItem()
    {
        InventoryItem pickUpItem = itemInSlot;
        if (pickUpItem != null)
        {
            inventory.SubtractStats(pickUpItem.itemData.stats);
            ClearSlot(pickUpItem);
        }

        return pickUpItem;
    }

    private void ClearSlot(InventoryItem pickUpItem)
    {
        itemInSlot = null;

        RectTransform rt = pickUpItem.GetComponent<RectTransform>();
        rt.SetParent(null);
    }
}
