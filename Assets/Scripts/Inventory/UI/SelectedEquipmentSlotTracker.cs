using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedEquipmentSlotTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    EquipmentItemSlot slot;

    [SerializeField] private TooltipController tooltipController;

    void Awake()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        slot = GetComponent<EquipmentItemSlot>();

        if (tooltipController == null)
            tooltipController = FindFirstObjectByType<TooltipController>();

        if (tooltipController == null)
            Debug.LogWarning("No TooltipController found in the scene!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = slot;

        var item = slot.GetItem();
        if (item != null)
        {
            tooltipController?.ShowHoverTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;

        var selectedItemController = FindFirstObjectByType<SelectedItemController>();
        if (selectedItemController == null || !selectedItemController.HasItem)
        {
            var item = slot.GetItem();
            if (item != null)
                tooltipController?.ClearHoverTooltip(slot.gameObject);
        }
    }
}
