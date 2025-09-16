using UnityEngine;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private ItemTooltip tooltip;
    [SerializeField] private SelectedItemController selectedItemController;

    private GameObject currentHoverTarget;

    public void ShowHoverTooltip(InventoryItem item, bool followMouse = true)
    {
        if (item == null) return;
        currentHoverTarget = item.gameObject;
        Debug.Log($"[TooltipController] ShowHoverTooltip - Item: {item.name}, followMouse: {followMouse}");
        tooltip?.ShowForItem(item, followMouse, item.gameObject);
    }

    public void ShowHoverTooltip(ItemData itemData, GameObject target, bool followMouse = true)
    {
        if (itemData == null || target == null) return;
        currentHoverTarget = target;
        Debug.Log($"[TooltipController] ShowHoverTooltip - ItemData: {itemData.name}, followMouse: {followMouse}");
        tooltip?.ShowForItemData(itemData, followMouse, target);
    }

    public void ClearHoverTooltip(GameObject target)
    {
        if (currentHoverTarget != target) return;

        Debug.Log($"[TooltipController] ClearHoverTooltip - Target: {target.name}");
        currentHoverTarget = null;

        if (selectedItemController != null && selectedItemController.HasItem)
        {
            Debug.Log($"[TooltipController] Showing selected item tooltip: {selectedItemController.SelectedItem.name}");
            tooltip?.ShowForItem(selectedItemController.SelectedItem, true, selectedItemController.SelectedItem.gameObject);
        }
        else
        {
            Debug.Log("[TooltipController] Force hiding tooltip");
            tooltip?.ForceHide();
        }
    }

    public void HideTooltipForUI()
    {
        Debug.Log("[TooltipController] HideTooltipForUI called, force hiding tooltip");
        currentHoverTarget = null;
        tooltip?.ForceHide();
    }
}
