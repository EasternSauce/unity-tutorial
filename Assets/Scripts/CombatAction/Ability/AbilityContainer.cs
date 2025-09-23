using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AbilityContainer
{
    public Ability ability;
    public float currentCooldown;

    public AbilityContainer(Ability ability)
    {
        this.ability = ability;
    }

    public float CooldownNormalized => 1f - currentCooldown / ability.cooldown;

    internal void Cooldown()
    {
        currentCooldown = ability.cooldown;
    }

    internal void ReduceCooldown(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
        }
    }
}
