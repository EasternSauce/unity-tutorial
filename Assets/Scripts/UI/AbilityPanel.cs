using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AbilityPanel : MonoBehaviour
{
    [SerializeField] List<AbilitySlotButton> slotButtons;
    public UnityEvent<int> onAbilityActivate;

    public void ActivateAbility_1(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(0);
        }
    }
    public void ActivateAbility_2(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(1);
        }
    }
    public void ActivateAbility_3(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(2);
        }
    }
    public void ActivateAbility_4(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(3);
        }
    }
    public void ActivateAbility_5(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(4);
        }
    }
    public void ActivateAbility_6(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            ActivateAbility(5);
        }
    }

    public void ActivateAbility(int abilitySlot)
    {
        Debug.Log("Activated ability Num:" + abilitySlot.ToString());
        onAbilityActivate?.Invoke(abilitySlot);
    }

    public void ActivatePotion(int potionSlot)
    {
        Debug.Log("Activated potion Num:" + potionSlot.ToString());
    }

    public void UpdateAbility(AbilityContainer ability, int abilitySlotId)
    {
        slotButtons[abilitySlotId].UpdateAbility(ability);
    }

    public void UpdateCooldown(float cooldownNormalized, int abilitySlotId)
    {
        slotButtons[abilitySlotId].UpdateCooldown(cooldownNormalized);
    }
}
