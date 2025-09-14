using System.Collections;
using UnityEngine;

public class BowAttackExecutor : AttackExecutor
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 15f;
    [SerializeField] private float arrowHeightOffset = 1.2f;
    [SerializeField] private float arrowSpawnProgress = 0.5f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine localCoroutine;

    private void Update()
    {
        UpdateTimers();
        UpdateAttackLock();
        UpdateMovementState();
    }

    public void HandleBowAttack(Command command)
    {
        if (!CanAttack()) return;

        Vector3 targetPos = GetMouseWorldPosition();
        StopMovementAndRotate(targetPos);
        PrepareWeapon();
        PlayAttackAnimation();
        StartArrowSpawnCoroutine(targetPos);
    }

    public override void ResetState()
    {
        base.ResetState();
        StopArrowCoroutine();
        ResetTimersAndLock();
        ResetAnimatorTrigger("BowAttack");
    }

    private bool CanAttack()
    {
        return cooldownTimer <= 0f && localCoroutine == null;
    }

    private void UpdateTimers()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (animationTimer > 0f) animationTimer -= Time.deltaTime;
    }

    private void UpdateAttackLock()
    {
        if (animationTimer <= 0f)
        {
            isAttackLocked = false;
            return;
        }

        float progress = 1f - (animationTimer / attackAnimationTime);
        if (!isAttackLocked && progress >= 0.3f && progress <= 0.6f) isAttackLocked = true;
        else if (isAttackLocked && progress > 0.6f) isAttackLocked = false;
    }

    private void UpdateMovementState()
    {
        SetAttackingState(isAttackLocked);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position + Vector3.up * arrowHeightOffset);
        if (plane.Raycast(ray, out float distance)) return ray.GetPoint(distance);
        return transform.position + transform.forward * 10f + Vector3.up * arrowHeightOffset;
    }

    private void StopMovementAndRotate(Vector3 targetPos)
    {
        StopMovement();
        RotateTowardsPoint(targetPos);
    }

    private void PrepareWeapon()
    {
        SetAttackingState(true);
        character.GetComponent<WeaponVisibilityController>()?.ResetLingerTimer();
    }

    private void PlayAttackAnimation()
    {
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
        TriggerAttackAnimation();
    }

    private void StartArrowSpawnCoroutine(Vector3 targetPos)
    {
        float delay = attackAnimationTime * arrowSpawnProgress;
        localCoroutine = StartCoroutine(SpawnArrowDelayed(targetPos, delay));
    }

    private IEnumerator SpawnArrowDelayed(Vector3 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrowAtPosition(targetPos);
        cooldownTimer = ApplyCooldown(defaultTimeToAttack);
        StopAndClearCoroutine(ref localCoroutine);
        isAttackLocked = false;
        SetAttackingState(false);
    }

    private void SpawnArrowAtPosition(Vector3 targetPos)
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = transform.position + Vector3.up * arrowHeightOffset + transform.forward * 0.5f;
        GameObject arrowObject = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Arrow arrowScript = arrowObject.GetComponent<Arrow>();
        if (arrowScript == null) { Destroy(arrowObject); return; }
        Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
        Vector3 dir = (flatTarget - spawnPos).normalized;
        arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
    }

    private void StopArrowCoroutine()
    {
        StopAndClearCoroutine(ref localCoroutine);
    }

    private void ResetTimersAndLock()
    {
        animationTimer = 0f;
        isAttackLocked = false;
        SetAttackingState(false);
    }

    private void ResetAnimatorTrigger(string triggerName)
    {
        ResetAnimatorTriggers(triggerName);
    }

    private void TriggerAttackAnimation()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;
        string trigger = null;
        if (type == WeaponType.Bow && AnimatorHasTrigger("BowAttack")) trigger = "BowAttack";
        else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";
        if (!string.IsNullOrEmpty(trigger)) animator.SetTrigger(trigger);
    }
}
