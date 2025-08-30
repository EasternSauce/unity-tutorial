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

public enum WeaponType
{
    None,
    Bow,
    OneHandedAxe,
    TwoHandedAxe
}

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public EquipmentSlot equipmentSlot;
    public int sizeWidth = 1;
    public int sizeHeight = 1;
    public List<StatsValue> stats;
    public Sprite icon;

    [Tooltip("Set only if this item is a weapon")]
    public WeaponType weaponType = WeaponType.None;
}
