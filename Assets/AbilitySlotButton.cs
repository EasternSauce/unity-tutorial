using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotButton : MonoBehaviour
{
    [SerializeField] Image abilityIcon;

    public void UpdateAbility(AbilityContainer ability)
    {
        abilityIcon.sprite = ability.ability.icon;
    }
}
