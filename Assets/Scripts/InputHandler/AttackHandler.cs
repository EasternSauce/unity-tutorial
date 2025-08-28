using System.Collections;
using CharacterCommand;
using UnityEngine;

public class AttackHandler : MonoBehaviour, ICommandHandle
{
    Character character;
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    float attackTimer;

    [SerializeField] float attackAnimationTime = 1f;
    float animationTimer;

    Animator animator;
    CharacterMovement characterMovement;
    CanMoveState canMoveState;

    Coroutine attackCoroutine;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        character = GetComponent<Character>();
        canMoveState = GetComponent<CanMoveState>();
    }

    private void Update()
    {
        AttackTimerTick();
        AnimationTimerTick();
        UpdateCanMoveState();
    }

    private void UpdateCanMoveState()
    {
        canMoveState.isAttacking = animationTimer > 0f;
    }

    private void AnimationTimerTick()
    {
        if (animationTimer > 0f)
            animationTimer -= Time.deltaTime;
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
        if (command == null || command.target == null) return;

        float distance = Vector3.Distance(transform.position, command.target.transform.position);
        float attackBuffer = 0.1f;

        Transform targetTransform = command.target.transform;
        RotateTowardsTarget(targetTransform);

        if (distance <= attackRange + attackBuffer)
        {
            characterMovement.Stop();
            characterMovement.Agent.isStopped = true;

            if (!CheckAttack()) return;

            RotateTowardsTarget(targetTransform, forceInstant: true);

            ResetAttackTimer();
            SetAnimationTimer();

            // Determine trigger based on current weapon
            string attackTrigger = "MeleeAttack"; // default
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            InventoryItem weapon = playerInventory?.CurrentWeapon;

            if (weapon != null && weapon.itemData.weaponType != WeaponType.None)
            {
                switch (weapon.itemData.weaponType)
                {
                    case WeaponType.Melee:
                        attackTrigger = "MeleeAttack";
                        break;
                    case WeaponType.Bow:
                        attackTrigger = "BowAttack";
                        break;
                }
            }

            animator.SetTrigger(attackTrigger);

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
        }
    }

    private IEnumerator DelayedDamage(Command command)
    {
        float hitTime = attackAnimationTime * 0.4f;
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
        animator.ResetTrigger("MeleeAttack");
        animator.ResetTrigger("BowAttack");

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}
