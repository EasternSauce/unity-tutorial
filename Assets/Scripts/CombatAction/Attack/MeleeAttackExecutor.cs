using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float defaultCooldown = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float attackTimer;
    private float phaseTimer;
    private GameObject currentTarget;
    private bool hasDealtDamage;
    private AttackPhase currentPhase = AttackPhase.None;

    private enum AttackPhase { None, Windup, Damage }

    public MeleeAttackExecutor(Character character, MoveCommandHandler movement, Animator animator)
        : base(character, movement, animator) { }

    public override void Execute(Command command)
    {
        if (command == null || command.target == null || character == null || character.IsDead) return;

        if (currentTarget == command.target && currentPhase != AttackPhase.None)
            return;

        currentTarget = command.target;
        currentPhase = AttackPhase.None;
    }

    public override void TickUpdate()
    {
        if (character == null || character.IsDead || currentTarget == null) return;

        float distance = Vector3.Distance(character.transform.position, currentTarget.transform.position);
        float effectiveRange = attackRange;

        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        if (weapon == null || weapon.itemData.weaponType == WeaponType.None) effectiveRange = 1.5f;

        if (distance > effectiveRange)
        {
            MoveTowardsTarget(effectiveRange);
        }
        else
        {
            StopMovement();
            RotateTowardsPoint(currentTarget.transform.position);
            if (attackTimer <= 0f && currentPhase == AttackPhase.None)
                StartAttackPhase();
        }

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (phaseTimer > 0f) phaseTimer -= Time.deltaTime;

        if (currentPhase == AttackPhase.Windup && phaseTimer <= attackAnimationTime * 0.6f)
            ExecuteDamageOnTarget();
        else if (currentPhase == AttackPhase.Damage && phaseTimer <= 0f)
            EndAttackPhase();

        SetPerformingCombatAction(currentPhase != AttackPhase.None);
    }

    private void MoveTowardsTarget(float stopDistance)
    {
        if (movement != null && movement.Agent != null && movement.Agent.enabled && movement.Agent.isOnNavMesh)
        {
            movement.Agent.stoppingDistance = stopDistance;
            movement.SetDestination(currentTarget.transform.position);
            movement.Agent.isStopped = false;
        }
    }

    private void StartAttackPhase()
    {
        hasDealtDamage = false;
        currentPhase = AttackPhase.Windup;
        phaseTimer = attackAnimationTime;
        TriggerAttackAnimation();
    }

    private void ExecuteDamageOnTarget()
    {
        if (hasDealtDamage || currentTarget == null || character == null || character.IsDead) return;

        if (currentTarget.TryGetComponent<IDamageable>(out var damageable))
        {
            if (!(damageable is Character c) || !c.IsDead)
            {
                damageable.TakeDamage(character.GetDamage());

                if (damageable is Character deadChar && deadChar.IsDead)
                {
                    currentTarget = null;
                    currentPhase = AttackPhase.None;
                    return;
                }
            }
        }

        hasDealtDamage = true;
        attackTimer = ApplyCooldown(defaultCooldown);
        currentPhase = AttackPhase.Damage;
    }

    private void EndAttackPhase()
    {
        currentPhase = AttackPhase.None;
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        string trigger = null;
        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack")) trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack")) trigger = "TwoHandedMeleeAttack";
        else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);
    }

    public void CancelCurrentAttack()
    {
        currentTarget = null;
        currentPhase = AttackPhase.None;
        hasDealtDamage = false;
    }

    public override void ResetState()
    {
        CancelCurrentAttack();
        attackTimer = 0f;
        phaseTimer = 0f;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }
}
