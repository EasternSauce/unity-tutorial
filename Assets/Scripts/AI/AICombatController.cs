using UnityEngine;

public class AICombatController : MonoBehaviour
{
    private MoveCommandHandler moveHandler;
    private AttackCommandHandler attackHandler;
    private TargetHandler targetHandler;
    private AggroController aggroController;
    private Character selfCharacter;

    private void Awake()
    {
        moveHandler = GetComponent<MoveCommandHandler>();
        attackHandler = GetComponent<AttackCommandHandler>();
        targetHandler = GetComponent<TargetHandler>();
        aggroController = GetComponent<AggroController>();
        selfCharacter = GetComponent<Character>();
    }

    private void Update()
    {
        if (!CanAct())
        {
            aggroController.DropAggro();
            return;
        }

        if (targetHandler.HasValidTarget())
        {
            HandleTarget();
        }
        else
        {
            targetHandler.SearchForTargets();
        }
    }

    private bool CanAct() => selfCharacter != null && !selfCharacter.IsDead;

    private void HandleTarget()
    {
        if (aggroController.IsAggroed)
        {
            attackHandler?.ProcessCommand(new Command(CommandType.Attack, aggroController.CurrentTarget));
        }
    }

    public void OnAttacked(GameObject attacker)
    {
        if (attacker == null || aggroController.CurrentTarget == attacker) return;
        aggroController.GainAggro(attacker);
    }
}
