using CharacterCommand;
using UnityEngine;

public class AttackHandler : MonoBehaviour, ICommandHandle
{
    Character character;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float defaultTimeToAttack = 2f;
    float attackTimer;
    Animator animator;
    CharacterMovement characterMovement;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterMovement = GetComponent<CharacterMovement>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        AttackTimerTick();
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

        attackTime /= character.TakeStats(Statistic.AttackSpeed).float_value;

        return attackTime;
    }

    public void ProcessCommand(Command command)
    {
        float distance = Vector3.Distance(transform.position, command.target.transform.position);

        if (distance < attackRange)
        {
            if (attackTimer > 0f) { return; }

            attackTimer = GetAttackTime();

            characterMovement.Stop();
            animator.SetTrigger("Attack");

            DealDamage(command);

            command.isComplete = true;
        }
        else
        {
            characterMovement.SetDestination(command.target.transform.position);
        }
    }

    private void DealDamage(Command command)
    {
        IDamageable target = command.target.GetComponent<IDamageable>();
        int damage = character.GetDamage();
        target.TakeDamage(damage);
    }
}
