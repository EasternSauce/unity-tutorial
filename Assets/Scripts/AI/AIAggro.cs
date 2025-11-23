using System.Collections.Generic;
using UnityEngine;

public class AIAggro : MonoBehaviour
{
    [Header("Aggro Settings")]
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float aggroLoseDistance = 7f;
    [SerializeField] private float aggroLoseTime = 3f;

    [Header("Pack Aggro Settings")]
    [SerializeField] private float packAggroRadius = 10f;

    private float timeOutsideAggro;
    private bool isAggroed;
    public GameObject CurrentTarget { get; private set; }

    private AICombat aiCombat;

    private static int enemyLayerMask; // auto-assigned

    private void Awake()
    {
        aiCombat = GetComponent<AICombat>();

        if (enemyLayerMask == 0)
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            enemyLayerMask = 1 << enemyLayer;
        }
    }

    public bool HasTarget() => CurrentTarget != null;

    public void GainAggro(GameObject target)
    {
        if (target == null) return;

        CurrentTarget = target;
        isAggroed = true;
        timeOutsideAggro = 0f;

        AggroNearbyAllies(target);
    }

    private void AggroNearbyAllies(GameObject target)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, packAggroRadius, enemyLayerMask);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue; // skip self

            AIAggro ally = hit.GetComponentInParent<AIAggro>();
            if (ally == null) continue;

            if (!ally.HasTarget())
                ally.GainAggro(target);
        }
    }

    public void DropAggro()
    {
        CurrentTarget = null;
        isAggroed = false;
        timeOutsideAggro = 0f;
    }

    public bool IsTargetValid()
    {
        if (CurrentTarget == null) return false;

        var targetCharacter = CurrentTarget.GetComponent<Character>();

        if (targetCharacter == null || targetCharacter.IsDead)
        {
            DropAggro();
            return false;
        }

        return true;
    }

    public bool UpdateAggroTimerIfOutOfRange()
    {
        if (CurrentTarget == null) return false;

        float distance = DistanceHelper.Distance(transform.position, CurrentTarget.transform.position);

        float effectiveLoseDistance = aggroLoseDistance;

        if (aiCombat != null &&
            (aiCombat.WeaponType == AIWeaponType.Bow || aiCombat.WeaponType == AIWeaponType.Magic))
        {
            effectiveLoseDistance *= 1.5f;
        }

        if (distance > effectiveLoseDistance)
        {
            timeOutsideAggro += Time.deltaTime;
            if (timeOutsideAggro >= aggroLoseTime)
            {
                DropAggro();
                return false;
            }
        }
        else
        {
            timeOutsideAggro = 0f;
        }

        return true;
    }

    public bool ShouldAttack() => isAggroed;

    public float GetAggroDistance() => aggroDistance;
}
