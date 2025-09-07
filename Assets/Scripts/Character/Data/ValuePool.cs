using System;

[Serializable]
public class ValuePool
{
    public StatsValue maxValue;
    public int currentValue;

    public ValuePool(StatsValue maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = maxValue.integer_value;
    }

    public void FullRestore()
    {
        currentValue = maxValue.integer_value;
    }

    public void Restore(int value)
    {
        currentValue += value;
        if (currentValue > maxValue.integer_value)
            currentValue = maxValue.integer_value;
    }
}
