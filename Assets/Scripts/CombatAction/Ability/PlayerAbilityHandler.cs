using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerAbilityHandler : MonoBehaviour
{
    [SerializeField] private Ability startingAbility;
    private CombatActionController combatActionController;

    private List<AbilityContainer> abilities;

    public UnityEvent<AbilityContainer, int> onAbilityChange;
    public UnityEvent<float, int> onCooldownUpdate;

    private void Awake()
    {
        combatActionController = GetComponent<CombatActionController>();
    }

    private void Start()
    {
        abilities = new List<AbilityContainer>();
        AddAbility(startingAbility);
    }

    private void Update()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].ReduceCooldown(Time.deltaTime);
            onCooldownUpdate?.Invoke(abilities[i].CooldownNormalized, i);
        }
    }

    private void AddAbility(Ability abilityToAdd)
    {
        if (abilityToAdd == null) return;
        AbilityContainer container = new AbilityContainer(abilityToAdd);
        abilities.Add(container);
        onAbilityChange?.Invoke(container, abilities.Count - 1);
    }

    public void ActivateAbility(AbilityContainer ability)
    {
        if (ability.currentCooldown > 0f) return;
        if (ability.ability.name == "Fireball")
        {
            Vector3 targetPos = GetMouseWorldPosition();
            combatActionController.Execute(CombatActionType.Fireball, new Command(CommandType.CombatAction, targetPos));
        }
        ability.Cooldown();
    }

    public void ActivateAbility(int abilityId)
    {
        if (abilityId >= abilities.Count) return;
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
