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
        if (itemData == null || target == null) return;

        if (tooltip != null)
        {
            tooltip.SetCurrentTarget(target);
            tooltip.gameObject.SetActive(true);
            tooltip.ShowForItemData(itemData, false, target);

            var rt = tooltip.GetComponent<RectTransform>();
            rt.pivot = tooltip.InitialPivot;
            rt.anchoredPosition = tooltip.InitialAnchoredPosition;
        }

        currentHoverTarget = target;
    }

    public void ClearHoverTooltip(GameObject target)
    {
        if (currentHoverTarget != target) return;

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
