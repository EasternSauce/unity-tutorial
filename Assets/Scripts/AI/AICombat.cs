using UnityEngine;

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
        moveHandler?.Stop();
        combatActionController?.ResetAllExecutors();
    }

    private bool IsValidTarget(GameObject target)
    {
        return target != null && aggro.IsTargetValid() && aggro.UpdateAggroTimerIfOutOfRange();
    }

    private void MoveTowards(Vector3 position, float stoppingDistance)
    {
        moveHandler?.MoveTo(position, stoppingDistance);
    }

    private void StopMovement()
    {
        moveHandler?.Stop();
    }

    private void HandleMeleeCombatAction(GameObject target)
    {
        var meleeExecutor = combatActionController.GetExecutor<MeleeAttackExecutor>(CombatActionType.Melee);
        if (!CanEngageMelee(target, meleeExecutor))
            return;

        float attackRange = 1.5f;
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (ShouldMoveCloser(distance, attackRange))
            TryMoveTowardsTarget(target, attackRange);
        else
            EngageMeleeAttack(target, meleeExecutor);

        moveCommandTimer -= Time.deltaTime;
    }

    private bool CanEngageMelee(GameObject target, MeleeAttackExecutor meleeExecutor)
    {
        if (meleeExecutor == null || target == null)
            return false;

        if (target.TryGetComponent<Character>(out var targetChar) && targetChar.IsDead)
        {
            StopMovement();
            return false;
        }

        if (meleeExecutor.IsBusyAttacking())
        {
            StopMovement();
            return false;
        }

        return true;
    }

    private bool ShouldMoveCloser(float distance, float attackRange)
    {
        return distance > attackRange;
    }

    private void TryMoveTowardsTarget(GameObject target, float attackRange)
    {
        if (moveCommandTimer <= 0f || (target.transform.position - lastTargetPosition).sqrMagnitude > 0.01f)
        {
            MoveTowards(target.transform.position, attackRange * 0.95f);
            moveCommandTimer = moveCommandThrottle;
            lastTargetPosition = target.transform.position;
        }
        else
        {
            if (moveHandler.IsOnNavMesh)
                moveHandler.ResumeMovement();
        }
    }

    private void EngageMeleeAttack(GameObject target, MeleeAttackExecutor meleeExecutor)
    {
        StopMovement();
        if (!meleeExecutor.HasActiveTarget())
        {
            combatActionController.Execute(CombatActionType.Melee, new Command(CommandType.CombatAction, target));
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

        // Get correct executor
        bool isBusy = false;
        if (weaponType == AIWeaponType.Bow)
        {
            var bowExec = combatActionController.GetExecutor<BowAttackExecutor>(CombatActionType.Bow);
            if (bowExec != null && bowExec.IsBusyAttacking())
                isBusy = true;
        }
        else if (weaponType == AIWeaponType.Magic)
        {
            var fireballExec = combatActionController.GetExecutor<FireballAbilityExecutor>(CombatActionType.Fireball);
            if (fireballExec != null && fireballExec.IsBusyCasting())
                isBusy = true;
        }

        // 🔒 If currently attacking/casting, stand still and face target
        if (isBusy)
        {
            StopMovement();
            transform.LookAt(target.transform.position);
            return;
        }

        // Normal ranged behavior
        if (distance < minimumRangedDistance)
            StopMovement();
        else if (distance > preferredRangedDistance)
            MoveTowards(target.transform.position, preferredRangedDistance * 0.8f);
        else
            StopMovement();

        // Try to attack if in range
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
