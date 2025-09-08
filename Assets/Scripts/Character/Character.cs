using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isPlayer = false;
    public bool IsPlayer => isPlayer;

    [SerializeField] AttributeGroup attributes;
    [SerializeField] StatsGroup stats;
    public ValuePool lifePool;
    public ValuePool energyPool;

    private bool isDead;
    public bool IsDead => isDead;

    private float lifeRegen;

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

    private void LifeRegeneration()
    {
        if (isDead) return;

        lifeRegen += Time.deltaTime * stats.Get(Statistic.HealthRegeneration).float_value;
        if (lifeRegen > 1f)
        {
            Heal(1);
            lifeRegen -= 1f;
        }
    }

    private void Heal(int value)
    {
        if (isDead) return;
        lifePool.Restore(value);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        damage = ApplyDefence(damage);

        lifePool.currentValue -= damage;
        if (lifePool.currentValue < 0) lifePool.currentValue = 0;

        CheckDeath();
    }

    private int ApplyDefence(int damage)
    {
        damage -= stats.Get(Statistic.Armor).integer_value;
        return Mathf.Max(damage, 1);
    }

    private void CheckDeath()
    {
        if (!isDead && lifePool.currentValue <= 0)
        {
            isDead = true;
            lifePool.currentValue = 0;

            var handler = GetComponent<CommandHandler>();
            if (handler != null)
            {
                handler.SetCommand(null);
            }

            GetComponent<CharacterDefeatHandler>().Defeated();
        }
    }

    public StatsValue GetStatsValue(Statistic statisticToGet)
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
        foreach (var s in statsValues)
            stats.Sum(s);
    }

    public void SubtractStats(List<StatsValue> statsValues)
    {
        foreach (var s in statsValues)
            stats.Subtract(s);
    }

    public int GetDamage()
    {
        return GetStatsValue(Statistic.Damage).integer_value;
    }

    public ValuePool GetLifePool()
    {
        return lifePool;
    }

    public AttributeValue GetAttributeValue(Attribute attributeToShow)
    {
        return attributes.Get(attributeToShow);
    }
}
