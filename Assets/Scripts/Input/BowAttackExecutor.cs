using System.Collections;
using CharacterCommand;
using UnityEngine;

public class BowAttackExecutor : MonoBehaviour
{
    [Header("Bow Settings")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 15f;
    [SerializeField] float arrowHeightOffset = 1.2f;
    [SerializeField] float arrowSpawnProgress = 0.5f;

    Character character;
    CharacterMovement characterMovement;
    Animator animator;
    Coroutine attackCoroutine;

    private void Awake()
    {
        character = GetComponent<Character>();
        characterMovement = GetComponent<CharacterMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    public void HandleBowAttack(Command command, float attackAnimationTime,
        System.Action resetAttackTimer, System.Action setAnimationTimer,
        System.Action triggerAttackAnimation, ref Coroutine attackCoroutineRef)
    {
        if (command.isComplete) return;

        command.isComplete = true;

        characterMovement.Stop();
        if (characterMovement.Agent != null)
            characterMovement.Agent.isStopped = true;

        resetAttackTimer();
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

    private void RotateTowardsPoint(Vector3 point)
    {
        Vector3 lookVector = point - transform.position;
        lookVector.y = 0f;
        if (lookVector == Vector3.zero) return;

        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    public void ResetState()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}
