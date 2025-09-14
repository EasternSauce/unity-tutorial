using UnityEngine;

public static class HoverUtils
{
    public static void SetOutline(GameObject obj, bool enabled)
    {
        if (obj == null) return;
        var outline = obj.GetComponent<Outline>();
        if (outline != null) outline.enabled = enabled;
    }

    public static void SetTooltip(ItemTooltip tooltipUI, PickUpInteractableObject item)
    {
        if (tooltipUI == null || item == null || item.ItemData == null) return;
        string tooltipText = ItemTooltipBuilder.BuildTooltip(item.ItemData);
        Sprite icon = item.ItemData.icon;
        tooltipUI.Show(tooltipText, icon);
    }

    public static void HideTooltip(ItemTooltip tooltipUI)
    {
        tooltipUI?.Hide();
    }
}
