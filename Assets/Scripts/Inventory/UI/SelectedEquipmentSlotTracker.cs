using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedEquipmentSlotTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    EquipmentItemSlot slot;

    [SerializeField] private ItemTooltipController tooltipController;

    void Awake()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        slot = GetComponent<EquipmentItemSlot>();

        if (tooltipController == null)
        {
            tooltipController = FindFirstObjectByType<ItemTooltipController>();
            if (tooltipController == null)
                Debug.LogWarning("No TooltipController found in the scene!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = slot;

        var item = slot.GetItem();
        if (item != null)
        {
            tooltipController?.ShowTooltip(item, slot.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;
        tooltipController?.HideTooltip();
    }
}
