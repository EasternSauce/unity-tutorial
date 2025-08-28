using System;
using UnityEngine;

public class EquipmentItemSlot : MonoBehaviour
{
    [SerializeField] EquipmentSlot equipmentSlot;

    InventoryItem itemInSlot;

    RectTransform slotRectTransform;

    PlayerInventory inventory;

    private void Awake()
    {
        slotRectTransform = GetComponent<RectTransform>();
    }

    public void Init(PlayerInventory inventory)
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
        if (itemToPlace == null || itemToPlace.itemData == null)
        {
            Debug.LogError("Cannot place item: itemToPlace or its itemData is null.");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("Inventory is not initialized on EquipmentItemSlot.");
            return;
        }

        itemInSlot = itemToPlace;
        inventory.AddStats(itemInSlot.itemData.stats);

        RectTransform rt = itemToPlace.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.SetParent(slotRectTransform);
            rt.position = slotRectTransform.position;
        }
        else
        {
            Debug.LogWarning("Item does not have RectTransform.");
        }

        if (itemToPlace.itemData.equipmentSlot == EquipmentSlot.Weapon)
        {
            inventory.UpdateCurrentWeapon();
        }
    }

    public InventoryItem PickUpItem()
    {
        InventoryItem pickUpItem = itemInSlot;
        if (pickUpItem != null)
        {
            inventory.SubtractStats(pickUpItem.itemData.stats);
            ClearSlot(pickUpItem);
        }

        inventory.UpdateCurrentWeapon();

        return pickUpItem;
    }

    private void ClearSlot(InventoryItem pickUpItem)
    {
        itemInSlot = null;
        RectTransform rt = pickUpItem.GetComponent<RectTransform>();
        rt.SetParent(null);
    }

    public void HandleClick(SelectedItemController selectedItemController, ItemHighlightController itemHighlightController)
    {
        InventoryItem selectedItem = selectedItemController.SelectedItem;

        if (!selectedItemController.HasItem)
        {
            InventoryItem item = PickUpItem();
            if (item != null)
            {
                selectedItemController.PickUp(item);
                itemHighlightController?.SetSelectedItem(item);
            }
        }
        else
        {
            if (!Check(selectedItem)) return;

            InventoryItem replacedItem = ReplaceItem(selectedItem);

            if (replacedItem == null)
            {
                selectedItemController.ClearSelectedItem();
                itemHighlightController?.SetSelectedItem(null);
            }
            else
            {
                selectedItemController.SetSelectedItem(replacedItem);
                itemHighlightController?.SetSelectedItem(replacedItem);
            }
        }
    }

    public InventoryItem GetItem()
    {
        return itemInSlot;
    }
}
