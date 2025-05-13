using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemSlotInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    EquipmentItemSlot slot;

    void Awake()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        slot = GetComponent<EquipmentItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = slot;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemSlot = null;
    }
}
