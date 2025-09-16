using UnityEngine;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private SelectedItemController selectedItemController;

    private GameObject currentHoverTarget;

    public void ShowTooltipForGroundItem(ItemData data, GameObject target)
    {
        if (data == null) return;
        currentHoverTarget = target;
        tooltip?.ShowForItemData(data, false, target);
    }

    public void HideTooltipForGroundItem()
    {
        currentHoverTarget = null;
        tooltip?.ForceHide();
    }

    public void ShowTooltipForInventoryGridItem(InventoryItem item)
    {
        if (item == null) return;
        currentHoverTarget = item.gameObject;
        tooltip?.ShowForItem(item, true, item.gameObject);
    }

    public void HideTooltipForInventoryGridItem()
    {
        currentHoverTarget = null;
        tooltip?.ForceHide();
    }

    public void ShowTooltipForEquipmentSlot(InventoryItem item, GameObject slot)
    {
        if (item == null || slot == null) return;
        currentHoverTarget = slot;
        tooltip?.ShowForItem(item, true, slot);
    }

    public void HideTooltipForEquipmentSlot()
    {
        currentHoverTarget = null;
        tooltip?.ForceHide();
    }

    public void ForceHideTooltip()
    {
        currentHoverTarget = null;
        tooltip?.ForceHide();
    }
}
