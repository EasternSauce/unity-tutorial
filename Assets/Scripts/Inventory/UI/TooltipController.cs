using UnityEngine;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private SelectedItemController selectedItemController;

    private GameObject currentHoverTarget;

    public void ShowHoverTooltip(InventoryItem item)
    {
        currentHoverTarget = item.gameObject;
        tooltip?.ShowForItem(item, true, item.gameObject);
    }

    public void ShowHoverTooltip(ItemData itemData, GameObject target)
    {
        currentHoverTarget = target;
        tooltip?.ShowForItemData(itemData, true, target);
    }

    public void ClearHoverTooltip(GameObject target)
    {
        if (currentHoverTarget != target)
            return;

        currentHoverTarget = null;

        if (selectedItemController != null && selectedItemController.HasItem)
        {
            tooltip?.ShowForItem(selectedItemController.SelectedItem, true, selectedItemController.SelectedItem.gameObject);
        }
        else
        {
            tooltip?.ForceHide();
        }
    }
}
