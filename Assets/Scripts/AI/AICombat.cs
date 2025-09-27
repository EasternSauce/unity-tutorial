using UnityEngine;

public class AICombat : MonoBehaviour
{
    private AttackCommandHandler attackHandler;
    private MoveCommandHandler moveHandler;
    private AIAggro aggro;
    private CombatActionController combatActionController;

    [SerializeField] private AIWeaponType weaponType = AIWeaponType.Melee;
    [SerializeField] private float minimumRangedDistance = 3f;
    [SerializeField] private float moveCommandThrottle = 0.25f;

    public AIWeaponType WeaponType => weaponType;

    private float preferredRangedDistance = 10f;
    private float moveCommandTimer;
    private Vector3 lastTargetPosition;
    private bool attackCommandIssued;

    private void Awake()
    {
        attackHandler = GetComponent<AttackCommandHandler>();
        moveHandler = GetComponent<MoveCommandHandler>();
        aggro = GetComponent<AIAggro>();
        combatActionController = GetComponent<CombatActionController>();
    }

    private void Update()
    {
        if (aggro.HasTarget())
            HandleTarget(aggro.CurrentTarget);
    }

    public void HandleTarget(GameObject target)
    {
        if (target == null || !aggro.IsTargetValid() || !aggro.UpdateAggroTimerIfOutOfRange())
        {
            StopCombat();
            return;
        }

        if (weaponType == AIWeaponType.Melee)
            HandleMeleeTarget(target);
        else
            HandleRangedTarget(target);
    }

    private void HandleMeleeTarget(GameObject target)
    {
        if (combatActionController == null || moveHandler == null) return;

        var meleeExecutor = combatActionController.GetExecutor<MeleeAttackExecutor>(CombatActionType.Melee);
        if (meleeExecutor == null) return;

        // Absolute guard: freeze in place if attacking
        if (meleeExecutor.IsPerformingCombatAction)
        {
            moveHandler?.Stop();
            return;
        }

        float attackRange = 1.5f; // hardcoded
        float distance = Vector3.Distance(transform.position, target.transform.position);
        bool inRange = distance <= attackRange;

        if (!inRange)
        {
            moveCommandTimer -= Time.deltaTime;
            if (moveCommandTimer <= 0f || (target.transform.position - lastTargetPosition).sqrMagnitude > 0.01f)
            {
                moveHandler.SetDestination(target.transform.position);
                moveHandler.Agent.stoppingDistance = attackRange * 0.95f;
                moveHandler.Agent.isStopped = false;
                moveCommandTimer = moveCommandThrottle;
                lastTargetPosition = target.transform.position;
            }
            attackCommandIssued = false;
        }
        else
        {
            moveHandler?.Stop();

            if (!attackCommandIssued)
            {
                combatActionController.Execute(CombatActionType.Melee, new Command(CommandType.CombatAction, target));
                attackCommandIssued = true;
            }
        }
    }

    private void HandleRangedTarget(GameObject target)
    {
        if (combatActionController == null || moveHandler == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float effectiveLoseDistance = (weaponType == AIWeaponType.Bow || weaponType == AIWeaponType.Magic)
            ? aggro.GetAggroDistance() * 1.5f
            : aggro.GetAggroDistance();

        if (distance < minimumRangedDistance)
            moveHandler?.Stop();
        else if (distance > preferredRangedDistance)
        {
            if (moveHandler.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            {
                moveHandler.Agent.stoppingDistance = preferredRangedDistance * 0.8f;
                moveHandler.Agent.isStopped = false;
                moveHandler.SetDestination(target.transform.position);
            }
        }
        else
            moveHandler?.Stop();

        if (distance <= preferredRangedDistance)
        {
            switch (weaponType)
            {
                case AIWeaponType.Bow:
                    combatActionController.Execute(CombatActionType.Bow, new Command(CommandType.CombatAction, target));
                    break;
                case AIWeaponType.Magic:
                    combatActionController.Execute(CombatActionType.Fireball, new Command(CommandType.CombatAction, target));
                    break;
            }
        }
    }

    public void StopCombat()
    {
        moveHandler?.Stop();
        attackHandler?.CancelAttack();
        attackCommandIssued = false;
    }
}
