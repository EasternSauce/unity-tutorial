using UnityEngine;
using System.Collections.Generic;
using System;

public enum Statistic
{
    Life,
    Energy,
    Damage,
    Armor,
    AttackSpeed,
    MoveSpeed,
    HealthRegeneration
}

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

        if (toAdd.typeFloat == true)
        {
            statsValue.float_value += toAdd.float_value;
        }
        else
        {
            statsValue.integer_value += toAdd.integer_value;
        }
    }

    public void Subtract(StatsValue toSubtract)
    {
        StatsValue statsValue = stats[(int)toSubtract.statisticType];

        if (toSubtract.typeFloat == true)
        {
            statsValue.float_value -= toSubtract.float_value;
        }
        else
        {
            statsValue.integer_value -= toSubtract.integer_value;
        }
    }
}

public enum Attribute
{
    Strength,
    Dexterity,
    Intelligence
}

[Serializable]
public class AttributeValue
{
    public Attribute attributeType;
    public int value;

    public AttributeValue(Attribute attributeType, int value = 0)
    {
        this.attributeType = attributeType;
        this.value = value;
    }
}

[Serializable]
public class AttributeGroup
{
    public List<AttributeValue> attributeValues;

    public AttributeGroup()
    {
        attributeValues = new List<AttributeValue>();
    }

    public void Init()
    {
        attributeValues.Add(new AttributeValue(Attribute.Strength));
        attributeValues.Add(new AttributeValue(Attribute.Dexterity));
        attributeValues.Add(new AttributeValue(Attribute.Intelligence));
    }
}

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
        {
            currentValue = maxValue.integer_value;
        }
    }
}

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] AttributeGroup attributes;
    [SerializeField] StatsGroup stats;
    public ValuePool lifePool;
    public ValuePool energyPool;
    public bool isDead;

    private void Start()
    {
        attributes = new AttributeGroup();
        attributes.Init();

        stats = new StatsGroup();
        stats.Init();

        lifePool = new ValuePool(stats.Get(Statistic.Life));
        energyPool = new ValuePool(stats.Get(Statistic.Energy));

    }

    private void Update()
    {
        LifeRegeneration();
    }

    float lifeRegen;

    private void LifeRegeneration()
    {
        lifeRegen += Time.deltaTime * stats.Get(Statistic.HealthRegeneration).float_value;
        if (lifeRegen > 1f)
        {
            Heal(1);
            lifeRegen -= 1f;
        }
    }

    private void Heal(int value)
    {
        lifePool.Restore(value);
    }

    public void TakeDamage(int damage)
    {
        damage = ApplyDefence(damage);

        lifePool.currentValue -= damage;

        CheckDeath();
    }

    private int ApplyDefence(int damage)
    {
        damage -= stats.Get(Statistic.Armor).integer_value;

        if (damage <= 0)
        {
            damage = 1;
        }

        return damage;
    }

    private void CheckDeath()
    {
        if (lifePool.currentValue <= 0)
        {
            isDead = true;
            GetComponent<CharacterDefeatHandler>().Defeated();
        }
    }

    public StatsValue TakeStats(Statistic statisticToGet)
    {
        return stats.Get(statisticToGet);
    }

    public void Restore()
    {
        lifePool.FullRestore();
        isDead = false;
    }

    public void AddStats(List<StatsValue> statsValues)
    {
        for (int i = 0; i < statsValues.Count; i++)
        {
            StatsAdd(statsValues[i]);
        }
    }

    private void StatsAdd(StatsValue statsValue)
    {
        stats.Sum(statsValue);
    }

    public void SubtractStats(List<StatsValue> statsValues)
    {
        for (int i = 0; i < statsValues.Count; i++)
        {
            StatsSubstract(statsValues[i]);
        }
    }

    private void StatsSubstract(StatsValue statsValue)
    {
        stats.Subtract(statsValue);
    }

    public int GetDamage()
    {
        int damage = TakeStats(Statistic.Damage).integer_value;
        return damage;
    }

    public ValuePool GetLifePool()
    {
        return lifePool;
    }
}
