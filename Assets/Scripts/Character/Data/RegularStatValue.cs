using System;

[Serializable]
public class RegularStatValue
{
    public RegularStat statisticType;
    public bool typeFloat;
    public int integer_value;
    public float float_value;

    public RegularStatValue(RegularStat statisticType, int value = 0)
    {
        this.statisticType = statisticType;
        this.integer_value = value;
    }

    public RegularStatValue(RegularStat statisticType, float value = 0)
    {
        this.statisticType = statisticType;
        this.float_value = value;
        typeFloat = true;
    }
}
