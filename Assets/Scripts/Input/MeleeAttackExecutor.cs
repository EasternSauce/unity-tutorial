using System.Collections;
using UnityEngine;

public class MeleeAttackExecutor : AttackExecutor
{
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    [SerializeField] float attackAnimationTime = 1f;

    private float attackTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine localCoroutine;

    private CanMoveState canMoveState;
    private GameObject currentTarget;

    protected override void Awake()
    {
        base.Awake();
        canMoveState = GetComponent<CanMoveState>();
    }

    private void Update()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= 0.3f && progress <= 0.6f) isAttackLocked = true;
            else if (isAttackLocked && progress > 0.6f) isAttackLocked = false;
        }
        else
        {
            isAttackLocked = false;
        }

        if (canMoveState != null)
            canMoveState.isAttacking = isAttackLocked;
    }

    public void HandleMeleeAttack(Command command)
    {
        if (command.target == null) return;

        if (currentTarget != command.target)
        {
            CancelCurrentAttack();
        }
        else if (localCoroutine != null)
        {
            return;
        }

        currentTarget = command.target;
        localCoroutine = StartCoroutine(MeleeAttackRoutine(command));
    }

    private IEnumerator MeleeAttackRoutine(Command command)
    {
        Transform targetTransform = command.target.transform;

        while (command.target != null)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float range = attackRange;

            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
                range = 1.5f;

            if (distance <= range + 0.1f)
            {
                StopMovement();
                RotateTowardsTarget(targetTransform, true);

                if (attackTimer <= 0f)
                {
                    animationTimer = attackAnimationTime;
                    isAttackLocked = false;
                    TriggerAttackAnimation();

                    yield return new WaitForSeconds(attackAnimationTime * 0.4f);

                    if (command.target != null)
                    {
                        IDamageable target = command.target.GetComponent<IDamageable>();
                        if (target != null)
                            target.TakeDamage(character.GetDamage());
                    }

                    attackTimer = defaultTimeToAttack / character.GetStatsValue(Statistic.AttackSpeed).float_value;
                }
            }
            else
            {
                Vector3 dir = (targetTransform.position - transform.position).normalized;
                Vector3 destination = targetTransform.position - dir * range;
                characterMovement.Agent.stoppingDistance = 0f;
                characterMovement.Agent.isStopped = false;
                characterMovement.SetDestination(destination);
            }

            yield return null;
        }

        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        localCoroutine = null;
    }

    private void TriggerAttackAnimation()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        string trigger = null;

        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack"))
            trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack"))
            trigger = "TwoHandedMeleeAttack";
        else if (AnimatorHasTrigger("Attack"))
            trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack"))
            trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
        {
            animator.Update(0f);
            animator.SetTrigger(trigger);
        }
    }

    private bool AnimatorHasTrigger(string name)
    {
        foreach (var p in animator.parameters)
            if (p.type == UnityEngine.AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    public void CancelCurrentAttack()
    {
        if (localCoroutine != null)
        {
            StopCoroutine(localCoroutine);
            localCoroutine = null;
        }
        currentTarget = null;
    }

    public override void ResetState()
    {
        base.ResetState();
        CancelCurrentAttack();
        animationTimer = 0f;
        isAttackLocked = false;
        if (canMoveState != null) canMoveState.isAttacking = false;

        if (AnimatorHasTrigger("Attack")) animator.ResetTrigger("Attack");
        if (AnimatorHasTrigger("FistAttack")) animator.ResetTrigger("FistAttack");
        if (AnimatorHasTrigger("OneHandedMeleeAttack")) animator.ResetTrigger("OneHandedMeleeAttack");
        if (AnimatorHasTrigger("TwoHandedMeleeAttack")) animator.ResetTrigger("TwoHandedMeleeAttack");
    }
}
