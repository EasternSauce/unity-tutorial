using UnityEngine;
using System.Collections.Generic;

public class AggroController : MonoBehaviour
{
    [Header("Aggro Settings")]
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float aggroLoseDistance = 7f;
    [SerializeField] private float aggroLoseTime = 3f;

    private GameObject currentTarget;
    private float timeOutsideAggro;
    public bool IsAggroed { get; private set; }

    public GameObject CurrentTarget => currentTarget;

    private void Update()
    {
        if (currentTarget != null && IsTargetOutOfRange())
            UpdateAggroTimer();
        else
            ResetAggroTimer();
    }

    public void GainAggro(GameObject target)
    {
        currentTarget = target;
        IsAggroed = true;
        timeOutsideAggro = 0f;
    }

    public void DropAggro()
    {
        currentTarget = null;
        IsAggroed = false;
        timeOutsideAggro = 0f;
    }

    private bool IsTargetOutOfRange()
    {
        if (currentTarget == null) return false;
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance > aggroLoseDistance;
    }

    private void UpdateAggroTimer()
    {
        timeOutsideAggro += Time.deltaTime;
        if (timeOutsideAggro >= aggroLoseTime)
        {
            DropAggro();
        }
    }

    private void ResetAggroTimer() => timeOutsideAggro = 0f;

    public bool IsWithinAggroDistance(GameObject target)
    {
        if (target == null) return false;
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= aggroDistance;
    }
}
