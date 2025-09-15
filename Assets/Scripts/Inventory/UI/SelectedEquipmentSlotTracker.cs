using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedEquipmentSlotTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    EquipmentItemSlot slot;

    [SerializeField] private ItemTooltip tooltip;

    void Awake()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
        slot = GetComponent<EquipmentItemSlot>();

        if (tooltip == null)
        {
            tooltip = FindFirstObjectByType<ItemTooltip>();
            if (tooltip == null)
                Debug.LogWarning("No ItemTooltip found in the scene!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = slot;

        var item = slot.GetItem();
        if (item != null && item.itemData != null)
        {
            string tooltipText = ItemTooltipBuilder.BuildTooltip(item.itemData);
            tooltip?.Show(tooltipText, item.itemData.icon, true, item.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;
        if (tooltip != null && tooltip.CurrentTarget == slot.gameObject)
            tooltip.Hide();
    }
}
