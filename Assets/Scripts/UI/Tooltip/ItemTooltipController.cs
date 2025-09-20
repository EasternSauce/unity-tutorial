using UnityEngine;

public class ItemTooltipController : MonoBehaviour
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private SelectedItemController selectedItemController;


    public void ShowTooltip(ItemData data, GameObject target)
    {
        if (data == null) return;
        tooltip?.ShowForItemData(data, false, target);
    }

    public void ShowTooltip(InventoryItem item, GameObject target)
    {
        if (item == null) return;
        tooltip?.ShowForItem(item, true, target);
    }

    public void HideTooltip()
    {
        tooltip?.Hide();
    }
}
