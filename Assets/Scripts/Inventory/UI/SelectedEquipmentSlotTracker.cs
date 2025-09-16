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
        Debug.Log($"[EquipmentSlotTracker] OnPointerEnter - Slot: {slot.name}");

        var item = slot.GetItem();
        if (item != null)
        {
            Debug.Log($"[EquipmentSlotTracker] Showing tooltip for item: {item.name}");
            tooltipController?.ShowHoverTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;
        Debug.Log($"[EquipmentSlotTracker] OnPointerExit - Slot: {slot.name}");
        tooltipController?.HideTooltipForUI();
    }
}
