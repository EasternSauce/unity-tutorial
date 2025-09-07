using System;
using System.Collections.Generic;

[Serializable]
public class StatsGroup
{
    public List<StatsValue> stats;

    public StatsGroup()
    {
        stats = new List<StatsValue>();
    }

    public void Init()
    {
        stats.Add(new StatsValue(Statistic.Life, 100));
        stats.Add(new StatsValue(Statistic.Energy, 100));
        stats.Add(new StatsValue(Statistic.Damage, 15));
        stats.Add(new StatsValue(Statistic.Armor, 5));
        stats.Add(new StatsValue(Statistic.AttackSpeed, 0.4f));
        stats.Add(new StatsValue(Statistic.MoveSpeed, 2f));
        stats.Add(new StatsValue(Statistic.HealthRegeneration, 1f));
    }

    public StatsValue Get(Statistic statisticToGet)
    {
        return stats[(int)statisticToGet];
    }

    public void Sum(StatsValue toAdd)
    {
        StatsValue statsValue = stats[(int)toAdd.statisticType];
        if (toAdd.typeFloat)
            statsValue.float_value += toAdd.float_value;
        else
            statsValue.integer_value += toAdd.integer_value;
    }

    public void Subtract(StatsValue toSubtract)
    {
        StatsValue statsValue = stats[(int)toSubtract.statisticType];
        if (toSubtract.typeFloat)
            statsValue.float_value -= toSubtract.float_value;
        else
            statsValue.integer_value -= toSubtract.integer_value;
    }
}
