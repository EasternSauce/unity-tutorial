using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;

    private float cooldownTimer = 0f;
    private GameObject currentTarget;

    public MeleeAttackExecutor(Character character, MoveCommandHandler movement, Animator animator)
        : base(character, movement, animator)
    {
    }

    public override void Execute(Command command)
    {
        if (command == null || command.target == null || character == null || character.IsDead)
            return;

        currentTarget = command.target;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (currentTarget == null || character == null || character.IsDead)
        {
            return;
        }

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
            if (cooldownTimer <= 0f)
            {
                PerformAttack();
                cooldownTimer = attackCooldown;
            }
        }
    }

    private void PerformAttack()
    {
        TriggerAttackAnimation();
        if (currentTarget.TryGetComponent<IDamageable>(out var damageable))
        {
            int damage = Mathf.RoundToInt(character.GetDamage());
            damageable.TakeDamage(damage);
        }
        CancelCurrentAttackTargetOnly();
    }

    private void CancelCurrentAttackTargetOnly()
    {
        currentTarget = null;
    }

    public void CancelCurrentAttack()
    {
        CancelCurrentAttackTargetOnly();
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");
    }

    protected override void ResetState()
    {
        CancelCurrentAttack();
    }

    public override bool HasActiveTarget()
    {
        return currentTarget != null;
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown;
    }

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
    }

}
