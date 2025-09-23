using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAbilityHandler : MonoBehaviour
{
    [SerializeField] Ability startingAbility;
    [SerializeField] FireballAbilityExecutor fireballExecutor;

    private List<AbilityContainer> abilities;

    public UnityEvent<AbilityContainer, int> onAbilityChange;
    public UnityEvent<float, int> onCooldownUpdate;

    private void Start()
    {
        AddAbility(startingAbility);
    }

    private void AddAbility(Ability abilityToAdd)
    {
        if (abilities == null) abilities = new List<AbilityContainer>();

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
            onCooldownUpdate?.Invoke(abilities[i].CooldownNormalized, i);
        }
    }

    public void ActivateAbility(AbilityContainer ability)
    {
        if (ability.currentCooldown > 0f) return;

        if (ability.ability.name == "Fireball" && fireballExecutor != null)
        {
            Vector3 targetPos = GetMouseWorldPosition();
            fireballExecutor.CastFireballAtPosition(targetPos, gameObject);
        }

        ability.Cooldown();
    }

    public void ActivateAbility(int abilityId)
    {
        if (abilityId >= abilities.Count) return;
        if (abilities[abilityId] == null) return;

        ActivateAbility(abilities[abilityId]);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        if (plane.Raycast(ray, out float distance)) return ray.GetPoint(distance);
        return transform.position + transform.forward * 10f;
    }
}
