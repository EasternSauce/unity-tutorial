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
        {
            animationTimer -= Time.deltaTime;
        }
    }

    private void AttackTimerTick()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
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

        if (character != null && character.IsPlayer)
        {
            FaceTarget(command.target.transform);
        }

        if (distance < attackRange)
        {
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;

            if (!CheckAttack())
            {
                return;
            }

            FaceTarget(command.target.transform);

            ResetAttackTimer();
            SetAnimationTimer();
            characterMovement.Stop();
            animator.SetTrigger("Attack");

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }

            attackCoroutine = StartCoroutine(DelayedDamage(command));
        }
        else
        {
            characterMovement.Agent.stoppingDistance = attackRange - 0.1f;

            characterMovement.SetDestination(command.target.transform.position);
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
        if (currentDistance > attackRange)
        {
            Debug.Log("Attack missed: target moved out of range.");
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
        if (attackTimer > 0f) { return false; }
        return true;
    }

    private void FaceTarget(Transform target)
    {
        Vector3 lookVector = target.position - transform.position;
        lookVector.y = 0f;
        Quaternion quaternion = Quaternion.LookRotation(lookVector);
        transform.rotation = quaternion;
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
        animator.ResetTrigger("Attack");

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}
