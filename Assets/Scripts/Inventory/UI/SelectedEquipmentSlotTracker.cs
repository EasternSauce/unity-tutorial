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
        if (item != null)
        {
            tooltip?.ShowForItem(item, true, slot.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;

        var selectedItemController = FindFirstObjectByType<SelectedItemController>();
        if (selectedItemController == null || !selectedItemController.HasItem)
            tooltip?.HideIfTarget(slot.gameObject);
    }

}
