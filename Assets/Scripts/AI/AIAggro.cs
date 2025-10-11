using UnityEngine;

public class AIAggro : MonoBehaviour
{
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float aggroLoseDistance = 7f;
    [SerializeField] private float aggroLoseTime = 3f;

    private float timeOutsideAggro;
    private bool isAggroed;
    public GameObject CurrentTarget { get; private set; }

    private AICombat aiCombat;

    private void Awake()
    {
        aiCombat = GetComponent<AICombat>();
    }

    public bool HasTarget()
    {
        return CurrentTarget != null;
    }

    public void GainAggro(GameObject target)
    {
        CurrentTarget = target;
        isAggroed = true;
        timeOutsideAggro = 0f;
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
        if (aiCombat != null && (aiCombat.WeaponType == AIWeaponType.Bow || aiCombat.WeaponType == AIWeaponType.Magic))
            effectiveLoseDistance *= 1.5f;

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

    public bool ShouldAttack()
    {
        return isAggroed;
    }

    public float GetAggroDistance()
    {
        return aggroDistance;
    }
}
