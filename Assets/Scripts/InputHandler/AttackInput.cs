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

    public bool AttackCheck()
    {
        return interactInput.attackTarget != null && interactInput.hoveringOverObject != null;
    }
}
