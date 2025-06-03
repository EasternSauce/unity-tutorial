using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityPanel : MonoBehaviour
{
    [SerializeField] List<AbilitySlotButton> slotButtons;
    public UnityEvent<int> onAbilityActivate;

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
}
