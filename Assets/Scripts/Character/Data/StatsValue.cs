using System;

[Serializable]
public class StatsValue
{
    public Statistic statisticType;
    public bool typeFloat;
    public int integer_value;
    public float float_value;

    public StatsValue(Statistic statisticType, int value = 0)
    {
        this.statisticType = statisticType;
        this.integer_value = value;
    }

    public StatsValue(Statistic statisticType, float value = 0)
    {
        this.statisticType = statisticType;
        this.float_value = value;
        typeFloat = true;
    }
}
