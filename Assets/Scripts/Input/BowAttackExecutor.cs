using System.Collections;
using UnityEngine;

public class BowAttackExecutor : AttackExecutor
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 15f;
    [SerializeField] float arrowHeightOffset = 1.2f;
    [SerializeField] float arrowSpawnProgress = 0.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    [SerializeField] float attackAnimationTime = 1f;

    private float attackTimer;
    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine localCoroutine;
    private CanMoveState canMoveState;

    protected override void Awake()
    {
        base.Awake();
        canMoveState = GetComponent<CanMoveState>();
    }

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
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
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
        if (canMoveState != null) canMoveState.isAttacking = isAttackLocked;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position + Vector3.up * arrowHeightOffset);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return transform.position + transform.forward * 10f + Vector3.up * arrowHeightOffset;
    }

    private void StopMovementAndRotate(Vector3 targetPos)
    {
        StopMovement();
        RotateTowardsPoint(targetPos);
    }

    private void PrepareWeapon()
    {
        if (canMoveState != null) canMoveState.isAttacking = true;

        WeaponVisibilityController visibility = character.GetComponent<WeaponVisibilityController>();
        visibility?.ResetLingerTimer();
    }

    private void PlayAttackAnimation()
    {
        SetAnimationTimer();
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
        ResetCooldown();
        localCoroutine = null;
        isAttackLocked = false;
        if (canMoveState != null) canMoveState.isAttacking = false;
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

        Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
        Vector3 dir = (flatTarget - spawnPos).normalized;

        arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
    }

    private void SetAnimationTimer()
    {
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
    }

    private void ResetCooldown()
    {
        float atkSpeed = character.GetStatsValue(Statistic.AttackSpeed).float_value;
        cooldownTimer = defaultTimeToAttack / atkSpeed;
    }

    private void TriggerAttackAnimation()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;
        string trigger = null;

        if (type == WeaponType.Bow && AnimatorHasTrigger("BowAttack"))
            trigger = "BowAttack";

        if (string.IsNullOrEmpty(trigger))
        {
            if (AnimatorHasTrigger("Attack")) trigger = "Attack";
            else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";
        }

        if (!string.IsNullOrEmpty(trigger)) animator.SetTrigger(trigger);
    }

    private bool AnimatorHasTrigger(string name)
    {
        foreach (var p in animator.parameters)
            if (p.type == UnityEngine.AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    private void StopArrowCoroutine()
    {
        if (localCoroutine != null)
        {
            StopCoroutine(localCoroutine);
            localCoroutine = null;
        }
    }

    private void ResetTimersAndLock()
    {
        attackTimer = 0f;
        animationTimer = 0f;
        isAttackLocked = false;

        if (canMoveState != null) canMoveState.isAttacking = false;
    }

    private void ResetAnimatorTrigger(string triggerName)
    {
        if (AnimatorHasTrigger(triggerName)) animator.ResetTrigger(triggerName);
    }
}
