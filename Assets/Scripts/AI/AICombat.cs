using UnityEngine;

/*
AICombat.cs

Purpose:
- Manages AI combat behavior for characters (melee and ranged).

Functional requirements (must be preserved after any change):
- Dead targets must never be attacked.
- AI must respect attack cooldowns and windup timers.
- Melee AI freezes movement while performing attacks.
- Ranged AI maintains minimum and preferred distances.
- Attack commands should only be issued when the AI is ready to perform the action:
    - Melee: executor is idle and target in range.
    - Ranged: cooldown complete, AI in position, and attack animation allows projectile spawn.
- AI should stop combat if the target is no longer valid or leaves aggro range.
- **Do not remove or make public properties private unless you are certain they are not used externally**.  

Constraints / notes:
- Uses CombatActionController for all attack execution.
- Movement commands must respect NavMeshAgent capabilities.
- Handles both melee and ranged weapon types separately.
*/


public class AICombat : MonoBehaviour
{
    [SerializeField] private AIWeaponType weaponType = AIWeaponType.Melee;
    public AIWeaponType WeaponType => weaponType;

    [SerializeField] private float minimumRangedDistance = 3f;
    [SerializeField] private float preferredRangedDistance = 10f;
    [SerializeField] private float moveCommandThrottle = 0.25f;

    private CombatActionController combatActionController;
    private MoveCommandHandler moveHandler;
    private AIAggro aggro;

    private float moveCommandTimer;
    private Vector3 lastTargetPosition;
    private bool attackCommandIssued;

    private void Awake()
    {
        combatActionController = GetComponent<CombatActionController>();
        moveHandler = GetComponent<MoveCommandHandler>();
        aggro = GetComponent<AIAggro>();
    }

    private void Update()
    {
        if (!aggro.HasTarget()) return;

        GameObject target = aggro.CurrentTarget;
        HandleTarget(target);
    }

    public void HandleTarget(GameObject target)
    {
        if (!IsValidTarget(target))
        {
            StopCombat();
            return;
        }

        switch (weaponType)
        {
            case AIWeaponType.Melee:
                HandleMeleeCombatAction(target);
                break;
            case AIWeaponType.Bow:
            case AIWeaponType.Magic:
                HandleRangedCombatAction(target);
                break;
        }
    }

    public void StopCombat()
    {
        StopMovement();
        combatActionController?.ResetAllExecutors();
        attackCommandIssued = false;
    }

    private bool IsValidTarget(GameObject target)
    {
        return target != null && aggro.IsTargetValid() && aggro.UpdateAggroTimerIfOutOfRange();
    }

    private void MoveTowards(Vector3 position, float stoppingDistance)
    {
        if (moveHandler?.Agent == null || !moveHandler.Agent.enabled || !moveHandler.Agent.isOnNavMesh)
            return;

        moveHandler.Agent.stoppingDistance = stoppingDistance;
        moveHandler.SetDestination(position);
        moveHandler.Agent.isStopped = false;
    }

    private void StopMovement()
    {
        moveHandler?.Stop();
        if (moveHandler?.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            moveHandler.Agent.isStopped = true;
    }

    private void HandleMeleeCombatAction(GameObject target)
    {
        var meleeExecutor = combatActionController.GetExecutor<MeleeAttackExecutor>(CombatActionType.Melee);
        if (meleeExecutor == null) return;

        if (target.TryGetComponent<Character>(out var targetChar) && targetChar.IsDead)
        {
            StopMovement();
            attackCommandIssued = false;
            return;
        }

        if (meleeExecutor.IsPerformingCombatAction)
        {
            StopMovement();
            return;
        }

        float attackRange = 1.5f;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        bool inRange = distance <= attackRange;

        if (!inRange)
        {
            moveCommandTimer -= Time.deltaTime;
            if (moveCommandTimer <= 0f || (target.transform.position - lastTargetPosition).sqrMagnitude > 0.01f)
            {
                MoveTowards(target.transform.position, attackRange * 0.95f);
                moveCommandTimer = moveCommandThrottle;
                lastTargetPosition = target.transform.position;
            }
            attackCommandIssued = false;
        }
        else
        {
            StopMovement();
            if (!attackCommandIssued)
            {
                combatActionController.Execute(CombatActionType.Melee, new Command(CommandType.CombatAction, target));
                attackCommandIssued = true;
            }
        }
    }

    private void HandleRangedCombatAction(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (target.TryGetComponent<Character>(out var targetChar) && targetChar.IsDead)
        {
            StopMovement();
            return;
        }

        if (distance < minimumRangedDistance)
        {
            StopMovement();
        }
        else if (distance > preferredRangedDistance)
        {
            MoveTowards(target.transform.position, preferredRangedDistance * 0.8f);
        }
        else
        {
            StopMovement();
        }

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
}
