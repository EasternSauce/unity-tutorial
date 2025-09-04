using System.Collections.Generic;
using UnityEngine;

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
