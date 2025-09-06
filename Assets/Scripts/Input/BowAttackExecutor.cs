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

    public void HandleBowAttack(Command command, float attackAnimationTime,
        System.Action resetAttackTimer, System.Action setAnimationTimer,
        System.Action triggerAttackAnimation, ref Coroutine attackCoroutineRef)
    {
        if (command.isComplete) return;
        command.isComplete = true;

        StopMovement();
        setAnimationTimer();
        triggerAttackAnimation();

        RotateTowardsPoint(command.worldPoint);

        if (attackCoroutineRef != null)
            StopCoroutine(attackCoroutineRef);

        float delay = attackAnimationTime * arrowSpawnProgress;
        attackCoroutineRef = StartCoroutine(SpawnArrowDelayed(command.worldPoint, delay));
        attackCoroutine = attackCoroutineRef;
    }

    private IEnumerator SpawnArrowDelayed(Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnArrowAtPosition(targetPos);

        AttackHandler attackHandler = GetComponent<AttackHandler>();
        attackHandler?.ResetAttackTimer();
    }


    private void SpawnArrowAtPosition(Vector3 mouseWorldPos)
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

        Vector3 dir = (mouseWorldPos - spawnPos).normalized;
        dir.y = 0f;

        arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
    }
}
