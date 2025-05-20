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
        InventoryItem replaceItem = itemInSlot;
        if (replaceItem != null)
        {
            inventory.SubtractStats(replaceItem.itemData.stats);
        }

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
}
