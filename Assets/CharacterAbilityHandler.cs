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

public class CharacterAbilityHandler : MonoBehaviour
{
    [SerializeField] Ability startingAbility;

    List<AbilityContainer> abilities;

    public UnityEvent<AbilityContainer, int> onAbilityChange;

    private void Start()
    {
        AddAbility(startingAbility);
    }

    private void AddAbility(Ability abilityToAdd)
    {
        if (abilities == null)
        {
            abilities = new List<AbilityContainer>();
        }

        AbilityContainer abilityContainer = new AbilityContainer(abilityToAdd);
        abilities.Add(abilityContainer);
        onAbilityChange?.Invoke(abilityContainer, abilities.Count - 1);
    }

    private void Update()
    {
        ProcessCooldown();
    }

    private void ProcessCooldown()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].ReduceCooldown(Time.deltaTime);
        }
    }

    public void ActivateAbility(AbilityContainer ability)
    {
        if (ability.currentCooldown > 0f) { return; }

        Debug.Log("Activate: " + ability.ability.name);
        ability.Cooldown();
    }

    public void ActivateAbility(int abilityId)
    {
        AbilityContainer abilityContainer = abilities[abilityId];
        ActivateAbility(abilityContainer);
    }
}
