using UnityEngine;

public class AttackInput : MonoBehaviour
{
    InteractInput interactInput;
    AttackHandler attackHandler;

    void Awake()
    {
        interactInput = GetComponent<InteractInput>();
        attackHandler = GetComponent<AttackHandler>();
    }

    public void Attack()
    {
        attackHandler.Attack(interactInput.hoveringOverCharacter);
    }

    public bool AttackCheck()
    {
        return interactInput.hoveringOverCharacter != null;
    }
}
