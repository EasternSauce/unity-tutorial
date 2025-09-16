using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EquipmentSlot equipmentSlot;
    [SerializeField] private TooltipController tooltipController;

    private InventoryItem itemInSlot;
    private RectTransform slotRectTransform;
    private PlayerInventory inventory;

    private void Awake()
    {
        slotRectTransform = GetComponent<RectTransform>();
        if (tooltipController == null)
            tooltipController = FindFirstObjectByType<TooltipController>();
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

        // NEW: immediately show tooltip if cursor is already over this slot
        ShowTooltipIfCursorInside();
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

        // Hide tooltip if cursor is still over this slot
        if (tooltipController != null &&
            RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, Input.mousePosition))
        {
            tooltipController.HideTooltip();
        }
    }

    public void HandleClick(SelectedItemController selectedItemController, InventoryItemHighlightController itemHighlightController)
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

    // === Tooltip logic ===
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltipIfCursorInside();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipController?.HideTooltip();
    }

    private void ShowTooltipIfCursorInside()
    {
        if (itemInSlot == null || tooltipController == null) return;

        if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, Input.mousePosition))
        {
            tooltipController.ShowTooltip(itemInSlot, itemInSlot.gameObject);
        }
    }
}
