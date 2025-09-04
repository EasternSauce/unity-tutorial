using System.Collections;
using CharacterCommand;
using UnityEngine;

public class AttackHandler : MonoBehaviour, ICommandHandle
{
    Character character;

    [Header("Melee Settings")]
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float defaultTimeToAttack = 1f;

    [Header("Animation Settings")]
    [SerializeField] float attackAnimationTime = 1f;
    float attackTimer;
    float animationTimer;

    [SerializeField] float attackLockStart = 0.3f;
    [SerializeField] float attackLockEnd = 0.6f;
    bool isAttackLocked = false;

    Animator animator;
    CharacterMovement characterMovement;
    CanMoveState canMoveState;
    Coroutine attackCoroutine;
    PlayerInventory playerInventory;
    BowAttackExecutor bowAttackExecutor;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        character = GetComponent<Character>();
        canMoveState = GetComponent<CanMoveState>();
        playerInventory = GetComponent<PlayerInventory>();
        bowAttackExecutor = GetComponent<BowAttackExecutor>();
    }

    private void Update()
    {
        AttackTimerTick();
        AnimationTimerTick();
        UpdateCanMoveState();
    }

    private void UpdateCanMoveState()
    {
        canMoveState.isAttacking = isAttackLocked;
    }

    private void AnimationTimerTick()
    {
        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= attackLockStart && progress <= attackLockEnd)
                isAttackLocked = true;
            else if (isAttackLocked && progress > attackLockEnd)
                isAttackLocked = false;
        }
        else
        {
            isAttackLocked = false;
        }
    }

    private void AttackTimerTick()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    float GetAttackTime()
    {
        float attackTime = defaultTimeToAttack;
        attackTime /= character.GetStatsValue(Statistic.AttackSpeed).float_value;
        return attackTime;
    }

    public void ProcessCommand(Command command)
    {
        if (command == null || (command.target == null && command.commandType != CommandType.Attack))
            return;

        if (!CheckAttack())
            return;

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        bool isBow = weapon != null && weapon.itemData.weaponType == WeaponType.Bow;

        if (isBow)
        {
            bowAttackExecutor.HandleBowAttack(command, attackAnimationTime,
                ResetAttackTimer, SetAnimationTimer, TriggerAttackAnimation, ref attackCoroutine);
        }
        else
        {
            HandleMeleeAttack(command);
        }
    }

    private void HandleMeleeAttack(Command command)
    {
        if (command.target == null) return;

        float distance = Vector3.Distance(transform.position, command.target.transform.position);
        float attackBuffer = 0.1f;
        Transform targetTransform = command.target.transform;

        RotateTowardsTarget(targetTransform);

        if (distance <= attackRange + attackBuffer)
        {
            characterMovement.Stop();
            characterMovement.Agent.isStopped = true;

            if (!CheckAttack()) return;

            RotateTowardsTarget(targetTransform, true);

            ResetAttackTimer();
            SetAnimationTimer();
            TriggerAttackAnimation();

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(DelayedDamage(command));
        }
        else
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            Vector3 destination = targetTransform.position - direction * attackRange;

            characterMovement.Agent.stoppingDistance = 0f;
            characterMovement.Agent.isStopped = false;
            characterMovement.SetDestination(destination);

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private string GetAttackTrigger()
    {
        if (playerInventory != null)
        {
            InventoryItem weapon = playerInventory.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
            {
                if (AnimatorHasParameter("FistAttack", AnimatorControllerParameterType.Trigger))
                    return "FistAttack";
            }
            else if (weapon.itemData.weaponType == WeaponType.OneHandedAxe)
            {
                if (AnimatorHasParameter("OneHandedMeleeAttack", AnimatorControllerParameterType.Trigger))
                    return "OneHandedMeleeAttack";
            }
            else if (weapon.itemData.weaponType == WeaponType.TwoHandedAxe)
            {
                if (AnimatorHasParameter("TwoHandedMeleeAttack", AnimatorControllerParameterType.Trigger))
                    return "TwoHandedMeleeAttack";
            }
            else if (weapon.itemData.weaponType == WeaponType.Bow)
            {
                if (AnimatorHasParameter("BowAttack", AnimatorControllerParameterType.Trigger))
                    return "BowAttack";
            }
        }
        if (AnimatorHasParameter("Attack", AnimatorControllerParameterType.Trigger))
            return "Attack";

        return null;
    }

    private void TriggerAttackAnimation()
    {
        string attackTrigger = GetAttackTrigger();
        if (!string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }

    private bool AnimatorHasParameter(string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == paramName)
                return true;
        }
        return false;
    }

    private IEnumerator DelayedDamage(Command command, float delay = -1f)
    {
        float hitTime = attackAnimationTime * 0.4f;
        if (delay >= 0f) hitTime = delay;

        yield return new WaitForSeconds(hitTime);

        if (command == null || command.isComplete || command.target == null)
        {
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
            yield break;
        }

        float currentDistance = Vector3.Distance(transform.position, command.target.transform.position);
        float attackBuffer = 0.1f;

        if (currentDistance > attackRange + attackBuffer)
        {
            command.isComplete = true;
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
            yield break;
        }

        DealDamage(command);
        command.isComplete = true;
        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        attackCoroutine = null;
    }

    private void SetAnimationTimer()
    {
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
    }

    public bool CheckAttack()
    {
        return attackTimer <= 0f;
    }

    private void RotateTowardsTarget(Transform target, bool forceInstant = false)
    {
        if (target == null) return;

        Vector3 lookVector = target.position - transform.position;
        lookVector.y = 0f;
        if (lookVector == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookVector);

        bool isMoving = characterMovement.Agent.velocity.magnitude > 0.1f;
        bool attackReady = CheckAttack();

        if (forceInstant || attackReady || isMoving)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    }

    private void ResetAttackTimer()
    {
        attackTimer = GetAttackTime();
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
    }

    private void DealDamage(Command command)
    {
        IDamageable target = command.target.GetComponent<IDamageable>();
        int damage = character.GetDamage();
        target.TakeDamage(damage);
    }

    public void ResetState()
    {
        animationTimer = 0f;
        isAttackLocked = false;

        if (AnimatorHasParameter("FistAttack", AnimatorControllerParameterType.Trigger))
            animator.ResetTrigger("FistAttack");
        if (AnimatorHasParameter("BowAttack", AnimatorControllerParameterType.Trigger))
            animator.ResetTrigger("BowAttack");
        if (AnimatorHasParameter("TwoHandedMeleeAttack", AnimatorControllerParameterType.Trigger))
            animator.ResetTrigger("TwoHandedMeleeAttack");
        if (AnimatorHasParameter("Attack", AnimatorControllerParameterType.Trigger))
            animator.ResetTrigger("Attack");

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        bowAttackExecutor?.ResetState();
    }
}
