using UnityEngine;

public enum AIWeaponType
{
    Melee,
    Bow
}

public class AICombat : MonoBehaviour
{
    private AttackCommandHandler attackHandler;
    private MoveCommandHandler moveHandler;
    private AIAggro aggro;

    [SerializeField] private AIWeaponType weaponType = AIWeaponType.Melee;
    [SerializeField] private float minimumRangedDistance = 3f;
    [SerializeField] private float distanceAdjustCooldown = 1.5f;

    public AIWeaponType WeaponType => weaponType;

    private float preferredRangedDistance = 10f; // doubled from previous 5
    private float distanceAdjustTimer = 0f;

    private void Awake()
    {
        attackHandler = GetComponent<AttackCommandHandler>();
        moveHandler = GetComponent<MoveCommandHandler>();
        aggro = GetComponent<AIAggro>();
    }

    private void Update()
    {
        distanceAdjustTimer -= Time.deltaTime;
    }

    public void HandleTarget(GameObject target)
    {
        if (!aggro.IsTargetValid()) return;
        if (!aggro.UpdateAggroTimerIfOutOfRange()) return;

        if (aggro.ShouldAttack())
        {
            if (weaponType == AIWeaponType.Bow)
                AdjustDistance(target);

            if (weaponType == AIWeaponType.Bow)
                GetComponent<BowAttackExecutor>()?.HandleBowAttack(new Command(CommandType.Attack, target));
            else
                GetComponent<MeleeAttackExecutor>()?.HandleMeleeAttack(new Command(CommandType.Attack, target));
        }
        else
        {
            StopCombat();
        }
    }

    private void AdjustDistance(GameObject target)
    {
        if (target == null || moveHandler == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < minimumRangedDistance)
        {
            moveHandler?.Stop();
            return;
        }

        if (distance < preferredRangedDistance && distanceAdjustTimer <= 0f)
        {
            Vector3 dir = (transform.position - target.transform.position).normalized;
            Vector3 destination = target.transform.position + dir * preferredRangedDistance;

            if (moveHandler.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            {
                moveHandler.Agent.stoppingDistance = 0f;
                moveHandler.Agent.isStopped = false;
                moveHandler.SetDestination(destination);
            }

            distanceAdjustTimer = distanceAdjustCooldown;
        }
        else if (distance > preferredRangedDistance)
        {
            if (moveHandler.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            {
                moveHandler.Agent.stoppingDistance = 0f;
                moveHandler.Agent.isStopped = false;
                moveHandler.SetDestination(target.transform.position);
            }
        }
    }

    public void StopCombat()
    {
        moveHandler?.Stop();
        attackHandler?.CancelAttack();
    }
}
