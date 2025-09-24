using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float attackCooldown;
    private float attackTimer;
    private GameObject currentTarget;
    private AttackPhase currentPhase = AttackPhase.None;
    private float phaseTimer;
    private bool hasDealtDamage;

    private enum AttackPhase { None, Windup, Damage }

    private void Update()
    {
        if (attackCooldown > 0f) attackCooldown -= Time.deltaTime;
        if (phaseTimer > 0f) phaseTimer -= Time.deltaTime;

        if (currentTarget == null || character == null || character.IsDead)
        {
            currentPhase = AttackPhase.None;
            ResumeMovement();
            return;
        }

        Character targetCharacter = currentTarget.GetComponent<Character>();
        if (targetCharacter != null && targetCharacter.IsDead)
        {
            currentTarget = null;
            currentPhase = AttackPhase.None;
            ResumeMovement();
            return;
        }

        // Movement only allowed if NOT in attack animation
        if (currentPhase == AttackPhase.None)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            float range = attackRange;
            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None) range = 1.5f;

            if (distance > range)
                MoveTowardsTarget(currentTarget.transform.position, range);
            else if (distance <= range && attackCooldown <= 0f)
                StartAttack();
        }
        else
        {
            // Attack in progress → stop movement completely
            StopMovement();

            // Handle attack phases
            if (currentPhase == AttackPhase.Windup && phaseTimer <= attackAnimationTime * 0.6f)
                DealDamage();

            if (currentPhase == AttackPhase.Damage && phaseTimer <= 0f)
                EndAttack();
        }

        SetPerformingCombatAction(currentPhase != AttackPhase.None);
    }


    public override void Execute(Command command)
    {
        if (command.target == null || character == null || character.IsDead) return;

        Character targetCharacter = command.target.GetComponent<Character>();
        if (targetCharacter != null && targetCharacter.IsDead) return;

        currentTarget = command.target;
    }

    private void StartAttack()
    {
        hasDealtDamage = false;
        currentPhase = AttackPhase.Windup;
        phaseTimer = attackAnimationTime;
        TriggerAttackAnimation();
        StopMovement();
    }

    private void DealDamage()
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
                    ResumeMovement();
                    return;
                }
            }
        }

        hasDealtDamage = true;
        attackCooldown = ApplyCooldown(defaultTimeToAttack);
        currentPhase = AttackPhase.Damage;
    }

    private void EndAttack()
    {
        currentPhase = AttackPhase.None;
        ResumeMovement();
    }

    private void MoveTowardsTarget(Vector3 targetPos, float stopDistance)
    {
        if (characterMovement != null && characterMovement.Agent != null && characterMovement.Agent.enabled && characterMovement.Agent.isOnNavMesh)
        {
            characterMovement.Agent.stoppingDistance = stopDistance;
            characterMovement.Agent.isStopped = false;
            characterMovement.SetDestination(targetPos);
        }
    }

    private void ResumeMovement()
    {
        if (characterMovement != null && characterMovement.Agent != null && characterMovement.Agent.enabled && characterMovement.Agent.isOnNavMesh)
            characterMovement.Agent.isStopped = false;
    }

    private void TriggerAttackAnimation()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        string trigger = null;
        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack")) trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack")) trigger = "TwoHandedMeleeAttack";
        else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
        {
            animator.Update(0f);
            animator.SetTrigger(trigger);
        }
    }

    public override void ResetState()
    {
        currentTarget = null;
        currentPhase = AttackPhase.None;
        hasDealtDamage = false;
        attackCooldown = 0f;
        phaseTimer = 0f;
        SetPerformingCombatAction(false);
        ResumeMovement();
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }
}
