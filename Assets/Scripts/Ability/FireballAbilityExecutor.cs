using System.Collections;
using UnityEngine;

public class FireballAbilityExecutor : BaseAttackExecutor
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 15f;
    [SerializeField] private float fireballHeightOffset = 1.2f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;
    [SerializeField] private float attackSpawnProgress = 0.5f;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine localCoroutine;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (animationTimer > 0f) animationTimer -= Time.deltaTime;

        if (animationTimer <= 0f) isAttackLocked = false;
        else
        {
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= 0.3f && progress <= 0.6f) isAttackLocked = true;
            else if (isAttackLocked && progress > 0.6f) isAttackLocked = false;
        }

        SetAttackingState(isAttackLocked);
    }

    public void CastFireball(Vector3 targetPos)
    {
        if (cooldownTimer > 0f || localCoroutine != null) return;

        StopMovement();
        RotateTowardsPoint(targetPos);
        SetAttackingState(true);

        animationTimer = attackAnimationTime;
        isAttackLocked = false;
        TriggerAttackAnimation();

        float delay = attackAnimationTime * attackSpawnProgress;
        localCoroutine = StartCoroutine(SpawnFireballDelayed(targetPos, delay));
    }

    private IEnumerator SpawnFireballDelayed(Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = transform.position + Vector3.up * fireballHeightOffset + transform.forward * 0.5f;
        Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
        Vector3 dir = (flatTarget - spawnPos).normalized;

        if (fireballPrefab != null)
        {
            GameObject proj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
            Fireball fireball = proj.GetComponent<Fireball>();
            if (fireball != null)
            {
                fireball.Initialize(character, dir, fireballSpeed, fireballHeightOffset);
            }
        }

        cooldownTimer = ApplyCooldown(defaultTimeToAttack);
        StopAndClearCoroutine(ref localCoroutine);
        isAttackLocked = false;
        SetAttackingState(false);
    }

    private void TriggerAttackAnimation()
    {
        if (AnimatorHasTrigger("BowAttack")) animator.SetTrigger("BowAttack");
        else if (AnimatorHasTrigger("Attack")) animator.SetTrigger("Attack");
        else if (AnimatorHasTrigger("FistAttack")) animator.SetTrigger("FistAttack");
    }

    public override void ResetState()
    {
        base.ResetState();
        StopAndClearCoroutine(ref localCoroutine);
        animationTimer = 0f;
        isAttackLocked = false;
        SetAttackingState(false);
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }
}
