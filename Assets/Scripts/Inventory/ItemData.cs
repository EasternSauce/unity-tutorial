using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlot
{
    None,
    Weapon,
    OffHand,
    Armor,
    Helmet,
    Belt,
    Boots,
    Ring,
    Amulet,
    Gloves
}

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public EquipmentSlot equipmentSlot;
    public int sizeWidth = 1;
    public int sizeHeight = 1;

    public List<StatsValue> stats;

    public Sprite icon;
}
