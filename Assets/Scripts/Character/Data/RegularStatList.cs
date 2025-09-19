using System;
using System.Collections.Generic;

[Serializable]
public class RegularStatList
{
    public List<RegularStatValue> regularStatValues;

    public RegularStatList()
    {
        regularStatValues = new List<RegularStatValue>();
    }

    public void Init()
    {
        regularStatValues.Add(new RegularStatValue(RegularStat.Life, 100));
        regularStatValues.Add(new RegularStatValue(RegularStat.Energy, 100));
        regularStatValues.Add(new RegularStatValue(RegularStat.Damage, 15));
        regularStatValues.Add(new RegularStatValue(RegularStat.Armor, 5));
        regularStatValues.Add(new RegularStatValue(RegularStat.AttackSpeed, 0.4f));
        regularStatValues.Add(new RegularStatValue(RegularStat.MoveSpeed, 2f));
        regularStatValues.Add(new RegularStatValue(RegularStat.HealthRegeneration, 1f));
    }

    public RegularStatValue Get(RegularStat statisticToGet)
    {
        return regularStatValues[(int)statisticToGet];
    }

    public void Sum(RegularStatValue toAdd)
    {
        RegularStatValue statsValue = regularStatValues[(int)toAdd.statisticType];
        if (toAdd.typeFloat)
            statsValue.float_value += toAdd.float_value;
        else
            statsValue.integer_value += toAdd.integer_value;
    }

    public void Subtract(RegularStatValue toSubtract)
    {
        RegularStatValue statsValue = regularStatValues[(int)toSubtract.statisticType];
        if (toSubtract.typeFloat)
            statsValue.float_value -= toSubtract.float_value;
        else
            statsValue.integer_value -= toSubtract.integer_value;
    }
}
