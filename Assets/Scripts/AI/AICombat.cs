using UnityEngine;

public enum AIWeaponType
{
    Melee,
    Bow,
    Magic
}

public class AICombat : MonoBehaviour
{
    private AttackCommandHandler attackHandler;
    private MoveCommandHandler moveHandler;
    private AIAggro aggro;
    private CombatActionController combatActionController;

    [SerializeField] private AIWeaponType weaponType = AIWeaponType.Melee;
    [SerializeField] private float minimumRangedDistance = 3f;

    public AIWeaponType WeaponType => weaponType;

    private float preferredRangedDistance = 10f;

    private void Awake()
    {
        attackHandler = GetComponent<AttackCommandHandler>();
        moveHandler = GetComponent<MoveCommandHandler>();
        aggro = GetComponent<AIAggro>();
        combatActionController = GetComponent<CombatActionController>();
    }

    public void HandleTarget(GameObject target)
    {
        if (!aggro.IsTargetValid()) return;
        if (!aggro.UpdateAggroTimerIfOutOfRange()) return;

        if (aggro.ShouldAttack())
        {
            if (weaponType == AIWeaponType.Bow || weaponType == AIWeaponType.Magic)
                AdjustDistance(target);

            switch (weaponType)
            {
                case AIWeaponType.Bow:
                    combatActionController.Execute(CombatActionType.Bow, new Command(CommandType.CombatAction, target));
                    break;
                case AIWeaponType.Magic:
                    combatActionController.Execute(CombatActionType.Fireball, new Command(CommandType.CombatAction, target));
                    break;
                case AIWeaponType.Melee:
                    combatActionController.Execute(CombatActionType.Melee, new Command(CommandType.CombatAction, target));
                    break;
            }
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

        if (distance > preferredRangedDistance)
        {
            if (moveHandler.Agent != null && moveHandler.Agent.enabled && moveHandler.Agent.isOnNavMesh)
            {
                moveHandler.Agent.stoppingDistance = preferredRangedDistance * 0.8f;
                moveHandler.Agent.isStopped = false;
                moveHandler.SetDestination(target.transform.position);
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
