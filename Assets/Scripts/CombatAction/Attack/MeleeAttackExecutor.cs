using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float defaultCooldown = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float phaseTimer;
    private float attackCooldownTimer;
    private GameObject currentTarget;
    private bool hasDealtDamage;
    private AttackPhase currentPhase = AttackPhase.None;
    private MoveCommandHandler movementHandler;

    private enum AttackPhase { None, Windup, Damage }

    public MeleeAttackExecutor(Character character, MoveCommandHandler movementHandler, Animator animator)
        : base(character, movementHandler, animator)
    {
        this.movementHandler = movementHandler;
    }

    public bool IsPerformingCombatAction => currentPhase != AttackPhase.None;

    public override void Execute(Command command)
    {
        if (command == null || command.target == null || character == null || character.IsDead)
            return;

        // If currently attacking the same target, cancel it
        if (currentTarget == command.target && currentPhase != AttackPhase.None)
        {
            CancelCurrentAttack();
            return;
        }

        // Only assign new target if not attacking
        if (currentPhase == AttackPhase.None && currentTarget == null)
        {
            currentTarget = command.target;
            hasDealtDamage = false;
        }
    }

    public override void TickUpdate()
    {
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        // HARD STOP: exit immediately if no target or dead
        if (currentTarget == null || character == null || character.IsDead)
        {
            ResetAttackState();
            return;
        }

        if (currentTarget.TryGetComponent<Character>(out var targetChar) && targetChar.IsDead)
        {
            ResetAttackState();
            return;
        }

        float distance = Vector3.Distance(character.transform.position, currentTarget.transform.position);
        float effectiveRange = GetEffectiveRange();

        if (distance > effectiveRange)
        {
            MoveTowardsTarget(effectiveRange);
        }
        else
        {
            StopMovement();
            RotateTowardsPoint(currentTarget.transform.position);

            if (attackCooldownTimer <= 0f && currentPhase == AttackPhase.None)
                StartAttackPhase();
        }

        if (phaseTimer > 0f)
            phaseTimer -= Time.deltaTime;

        if (currentPhase == AttackPhase.Windup && phaseTimer <= attackAnimationTime * 0.6f)
            ExecuteDamageOnTarget();
        else if (currentPhase == AttackPhase.Damage && phaseTimer <= 0f)
            EndAttackPhase();

        SetPerformingCombatAction(currentPhase != AttackPhase.None);
    }

    private void ResetAttackState()
    {
        if (currentPhase == AttackPhase.None)
            return;

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Resetting attack state. Previous phase: {currentPhase}");

        currentTarget = null;
        currentPhase = AttackPhase.None;
        hasDealtDamage = false;
        SetPerformingCombatAction(false);
    }

    private void MoveTowardsTarget(float stopDistance)
    {
        movementHandler?.MoveTo(currentTarget.transform.position, stopDistance);
    }

    private void StartAttackPhase()
    {
        if (currentTarget == null) return;

        hasDealtDamage = false;
        currentPhase = AttackPhase.Windup;
        phaseTimer = attackAnimationTime;
        attackCooldownTimer = ApplyCooldown(defaultCooldown);

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Starting attack phase (WINDUP) on target [{currentTarget.name}] with cooldown {attackCooldownTimer:F2}s");

        TriggerAttackAnimation();
    }

    private void ExecuteDamageOnTarget()
    {
        if (hasDealtDamage || currentTarget == null || character == null || character.IsDead)
            return;

        float distance = Vector3.Distance(character.transform.position, currentTarget.transform.position);
        float effectiveRange = GetEffectiveRange();

        if (distance > effectiveRange)
        {
            if (character.GetComponent<PlayerInventory>())
                Debug.Log($"[{character.name}] Missed attack — target [{currentTarget.name}] out of range ({distance:F2} > {effectiveRange:F2}).");

            currentPhase = AttackPhase.Damage;
            hasDealtDamage = true;
            return;
        }

        if (currentTarget.TryGetComponent<IDamageable>(out var damageable))
        {
            if (character.GetComponent<PlayerInventory>())
                Debug.Log($"[{character.name}] Attempting to deal damage to [{currentTarget.name}]...");

            if (!(damageable is Character c) || !c.IsDead)
            {
                int damage = Mathf.RoundToInt(character.GetDamage());
                damageable.TakeDamage(damage);

                if (character.GetComponent<PlayerInventory>())
                    Debug.Log($"[{character.name}] Dealt {damage} damage to [{currentTarget.name}].");

                if (damageable is Character deadChar && deadChar.IsDead)
                {
                    if (character.GetComponent<PlayerInventory>())
                        Debug.Log($"[{character.name}] Target [{currentTarget.name}] has been killed.");

                    ResetAttackState();
                    return;
                }
            }
        }

        hasDealtDamage = true;
        currentPhase = AttackPhase.Damage;

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Damage phase started — damage dealt successfully.");
    }

    private void EndAttackPhase()
    {
        if (currentPhase == AttackPhase.None)
            return;

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Ending attack phase (from {currentPhase}).");

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
        {
            if (character.GetComponent<PlayerInventory>())
                Debug.Log($"[{character.name}] Triggering attack animation: {trigger}");

            animator.SetTrigger(trigger);
        }
    }

    public void CancelCurrentAttack()
    {
        if (currentPhase == AttackPhase.None) return;

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Attack canceled (reason: Interrupted by new command) during {currentPhase} phase. Target: {currentTarget?.name}");

        // HARD STOP: clear everything immediately
        currentTarget = null;
        currentPhase = AttackPhase.None;
        hasDealtDamage = false;
        attackCooldownTimer = 0f;

        // Reset animator triggers to prevent lingering animation
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");

        SetPerformingCombatAction(false);
    }

    protected override void ResetState()
    {
        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Resetting combat state.");

        CancelCurrentAttack();
        phaseTimer = 0f;
        SetPerformingCombatAction(false);
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        float modified = baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;

        if (character.GetComponent<PlayerInventory>())
            Debug.Log($"[{character.name}] Applying cooldown modifier. Base: {baseCooldown:F2}s → Modified: {modified:F2}s");

        return modified;
    }

    private float GetEffectiveRange()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
            return 1.5f;

        return attackRange;
    }
}
