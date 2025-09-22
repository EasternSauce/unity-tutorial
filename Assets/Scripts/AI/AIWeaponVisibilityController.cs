using UnityEngine;

public class AIWeaponVisibilityController : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField] private GameObject bowInHand;
    [SerializeField] private GameObject bowOnBack;
    [SerializeField] private GameObject axeInHand;
    [SerializeField] private GameObject axeOnBack;

    private AICombat aiCombat;
    private Character character;

    private void Awake()
    {
        aiCombat = GetComponent<AICombat>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (aiCombat == null || character == null) return;

        // Reset visibility safely
        if (bowInHand) bowInHand.SetActive(false);
        if (bowOnBack) bowOnBack.SetActive(false);
        if (axeInHand) axeInHand.SetActive(false);
        if (axeOnBack) axeOnBack.SetActive(false);

        bool keepInHand = character.isPerformingCombatAction;

        if (aiCombat.WeaponType == AIWeaponType.Bow)
        {
            if (keepInHand)
            {
                if (bowInHand) bowInHand.SetActive(true);
            }
            else
            {
                if (bowOnBack) bowOnBack.SetActive(true);
            }
        }
        else
        {
            if (keepInHand)
            {
                if (axeInHand) axeInHand.SetActive(true);
            }
            else
            {
                if (axeOnBack) axeOnBack.SetActive(true);
            }
        }
    }
}
