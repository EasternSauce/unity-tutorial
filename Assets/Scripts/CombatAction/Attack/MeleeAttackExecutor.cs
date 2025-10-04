using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float damageDelay = 0.3f;
    [SerializeField] private float attackAnimationTime = 1f; // New: duration of the attack animation

    private float cooldownTimer = 0f;
    private float damageTimer = 0f;
    private float attackTimer = 0f; // New: how long the attack animation is playing

    private GameObject currentTarget;
    private IDamageable pendingDamageTarget;

    // Updated: now uses attackTimer instead of cooldownTimer
    public bool IsBusyAttacking() => attackTimer > 0f;

    public MeleeAttackExecutor(Character character, MoveCommandHandler movement, Animator animator)
        : base(character, movement, animator) { }

    public override void Execute(Command command)
    {
        if (command == null || command.target == null || character == null || character.IsDead)
            return;

        currentTarget = command.target;
    }

    public override void TickUpdate()
    {
        // Update timers
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f && pendingDamageTarget != null)
            {
                int damage = Mathf.RoundToInt(character.GetDamage());
                pendingDamageTarget.TakeDamage(damage);
                pendingDamageTarget = null;
            }
        }

        // Early exits
        if (currentTarget == null || character == null || character.IsDead)
            return;

        if (currentTarget.TryGetComponent<Character>(out var targetChar) && targetChar.IsDead)
        {
            CancelCurrentAttack();
            return;
        }

        float distance = Vector3.Distance(character.transform.position, currentTarget.transform.position);
        if (distance > attackRange)
        {
            movement?.MoveTo(currentTarget.transform.position, attackRange);
        }
        else
        {
            movement?.Stop();
            FaceDirection(currentTarget.transform.position);

            // Only trigger a new attack if not attacking and cooldown expired
            if (cooldownTimer <= 0f && attackTimer <= 0f)
            {
                TriggerAttackAnimation();

                if (currentTarget.TryGetComponent<IDamageable>(out var damageable))
                {
                    pendingDamageTarget = damageable;
                    damageTimer = damageDelay;
                }

                // Reset timers
                cooldownTimer = attackCooldown;
                attackTimer = attackAnimationTime; // NEW: start attack animation timer
            }
        }
    }

    public void CancelCurrentAttack()
    {
        currentTarget = null;
        pendingDamageTarget = null;
        damageTimer = 0f;
        attackTimer = 0f; // NEW: reset attack timer
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");
    }

    protected override void ResetState() => CancelCurrentAttack();

    public override bool HasActiveTarget() => currentTarget != null;

    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        var weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        string trigger = null;
        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack")) trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack")) trigger = "TwoHandedMeleeAttack";
        else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);

        // Reset attack timer on animation start
        attackTimer = attackAnimationTime;
    }
}
