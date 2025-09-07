using System.Collections;
using CharacterCommand;
using UnityEngine;

public class BowAttackExecutor : AttackExecutor
{
    [Header("Bow Settings")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 15f;
    [SerializeField] float arrowHeightOffset = 1.2f;
    [SerializeField] float arrowSpawnProgress = 0.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    [SerializeField] float attackAnimationTime = 1f;

    private float attackTimer;
    private Coroutine localCoroutine;

    private void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    public void HandleBowAttack(Command command)
    {
        if (attackTimer > 0f) return;

        StopMovement();
        RotateTowardsPoint(command.worldPoint);
        TriggerAttackAnimation();

        float delay = attackAnimationTime * arrowSpawnProgress;

        if (localCoroutine != null) StopCoroutine(localCoroutine);
        localCoroutine = StartCoroutine(SpawnArrowDelayed(command.worldPoint, delay));

        ResetAttackTimer();
    }

    private IEnumerator SpawnArrowDelayed(Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrowAtPosition(targetPos);
        localCoroutine = null;
    }

    private void SpawnArrowAtPosition(Vector3 targetPos)
    {
        if (arrowPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * arrowHeightOffset + transform.forward * 0.5f;
        GameObject arrowObject = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Arrow arrowScript = arrowObject.GetComponent<Arrow>();

        if (arrowScript == null)
        {
            Destroy(arrowObject);
            return;
        }

        Vector3 dir = (targetPos - spawnPos).normalized;
        dir.y = 0f;
        arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
    }

    private void ResetAttackTimer()
    {
        float atkSpeed = character.GetStatsValue(Statistic.AttackSpeed).float_value;
        attackTimer = defaultTimeToAttack / atkSpeed;
    }

    private void TriggerAttackAnimation()
    {
        string trigger = AnimatorHasTrigger("BowAttack") ? "BowAttack" : (AnimatorHasTrigger("Attack") ? "Attack" : null);
        if (!string.IsNullOrEmpty(trigger)) animator.SetTrigger(trigger);
    }

    private bool AnimatorHasTrigger(string name)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    public override void ResetState()
    {
        base.ResetState();
        attackTimer = 0f;
        localCoroutine = null;
    }
}
