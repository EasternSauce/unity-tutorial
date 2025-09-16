using UnityEngine;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private SelectedItemController selectedItemController;


    public void ShowTooltipForGroundItem(ItemData data, GameObject target)
    {
        if (data == null) return;
        tooltip?.ShowForItemData(data, false, target);
    }

    public void HideTooltipForGroundItem()
    {
        tooltip?.ForceHide();
    }

    public void ShowTooltipForInventoryGridItem(InventoryItem item)
    {
        if (item == null) return;
        tooltip?.ShowForItem(item, true, item.gameObject);
    }

    public void HideTooltipForInventoryGridItem()
    {
        tooltip?.ForceHide();
    }

    public void ShowTooltipForEquipmentSlot(InventoryItem item, GameObject slot)
    {
        if (item == null || slot == null) return;
        tooltip?.ShowForItem(item, true, slot);
    }

    public void HideTooltipForEquipmentSlot()
    {
        tooltip?.ForceHide();
    }
}
