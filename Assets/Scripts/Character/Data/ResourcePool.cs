using System;

[Serializable]
public class ResourcePool
{
    public RegularStatValue maxValue;
    public int currentValue;

    public ResourcePool(RegularStatValue maxValue)
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
