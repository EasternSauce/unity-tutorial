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
    [SerializeField] private float preferredRangedDistance = 5f;

    private void Awake()
    {
        attackHandler = GetComponent<AttackCommandHandler>();
        moveHandler = GetComponent<MoveCommandHandler>();
        aggro = GetComponent<AIAggro>();
    }

    public void HandleTarget(GameObject target)
    {
        if (!aggro.IsTargetValid()) return;
        if (!aggro.UpdateAggroTimerIfOutOfRange()) return;

        if (aggro.ShouldAttack())
        {
            if (weaponType == AIWeaponType.Bow)
                KeepDistanceFromTarget(target);

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

    private void KeepDistanceFromTarget(GameObject target)
    {
        if (target == null || moveHandler == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < preferredRangedDistance)
        {
            Vector3 dir = (transform.position - target.transform.position).normalized;
            Vector3 destination = target.transform.position + dir * preferredRangedDistance;

            if (moveHandler.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            {
                moveHandler.Agent.stoppingDistance = 0f;
                moveHandler.Agent.isStopped = false;
                moveHandler.SetDestination(destination);
            }
        }
        else
        {
            moveHandler?.Stop();
        }
    }

    public void StopCombat()
    {
        moveHandler?.Stop();
        attackHandler?.CancelAttack();
    }
}
