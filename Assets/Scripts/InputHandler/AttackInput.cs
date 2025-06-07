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

    public bool AttackCooldownCheck()
    {
        return attackHandler.CheckAttack();
    }

    public bool AttackTargetCheck()
    {
        return interactInput.attackTarget != null;// && interactInput.hoveringOverObject != null;
    }
}
