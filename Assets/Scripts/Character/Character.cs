using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isPlayer = false;
    public bool IsPlayer => isPlayer;

    [SerializeField] AttributeList attributes;
    [SerializeField] RegularStatList stats;
    public ResourcePool lifePool;
    public ResourcePool energyPool;

    private bool isDead;
    public bool IsDead => isDead;

    private float lifeRegen;

    private void Start()
    {
        attributes = new AttributeList();
        attributes.Init();

        stats = new RegularStatList();
        stats.Init();

        lifePool = new ResourcePool(stats.Get(RegularStat.Life));
        energyPool = new ResourcePool(stats.Get(RegularStat.Energy));
    }

    private void Update()
    {
        LifeRegeneration();
    }

    private void LifeRegeneration()
    {
        if (isDead) return;

        lifeRegen += Time.deltaTime * stats.Get(RegularStat.HealthRegeneration).float_value;
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
        damage -= stats.Get(RegularStat.Armor).integer_value;
        return Mathf.Max(damage, 1);
    }

    private void CheckDeath()
    {
        if (!isDead && lifePool.currentValue <= 0)
        {
            isDead = true;
            lifePool.currentValue = 0;

            var handler = GetComponent<CharacterCommandExecutor>();
            if (handler != null)
            {
                handler.ExecuteCommand(null);
            }

            GetComponent<CharacterDefeatHandler>().Defeated();
        }
    }

    public RegularStatValue GetStatsValue(RegularStat statisticToGet)
    {
        return stats.Get(statisticToGet);
    }

    public void Restore()
    {
        lifePool.FullRestore();
        isDead = false;
    }

    public void AddStats(List<RegularStatValue> statsValues)
    {
        foreach (var s in statsValues)
            stats.Sum(s);
    }

    public void SubtractStats(List<RegularStatValue> statsValues)
    {
        foreach (var s in statsValues)
            stats.Subtract(s);
    }

    public int GetDamage()
    {
        return GetStatsValue(RegularStat.Damage).integer_value;
    }

    public ResourcePool GetLifePool()
    {
        return lifePool;
    }

    public AttributeValue GetAttributeValue(Attribute attributeToShow)
    {
        return attributes.Get(attributeToShow);
    }
}
