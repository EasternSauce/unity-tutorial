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
    Amulet
}

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public EquipmentSlot equipmentSlot;
    public int sizeWidth = 1;
    public int sizeHeight = 1;

    public Sprite icon;
}
