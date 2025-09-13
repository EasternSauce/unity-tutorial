using UnityEngine;
using System.Text;

public static class ItemTooltipBuilder
{
    public static string BuildTooltip(ItemData itemData)
    {
        if (itemData == null) return "";

        StringBuilder tooltip = new StringBuilder();
        tooltip.AppendLine(itemData.name);

        if (itemData.stats != null && itemData.stats.Count > 0)
        {
            foreach (var stat in itemData.stats)
            {
                string valueText = stat.typeFloat ? stat.float_value.ToString("0.##") : stat.integer_value.ToString();
                tooltip.AppendLine($"{stat.statisticType}: {valueText}");
            }
        }

        if (itemData.equipmentSlot != EquipmentSlot.None)
            tooltip.AppendLine($"Slot: {itemData.equipmentSlot}");

        if (itemData.weaponType != WeaponType.None)
            tooltip.AppendLine($"Weapon: {itemData.weaponType}");

        return tooltip.ToString();
    }
}
