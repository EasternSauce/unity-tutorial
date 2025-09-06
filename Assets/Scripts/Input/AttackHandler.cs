using System.Collections;
using System.Collections.Generic;
using CharacterCommand;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CanMoveState))]
public class AttackHandler : MonoBehaviour, ICommandHandle
{
    private Character character;
    private Animator animator;
    private CanMoveState canMoveState;
    private PlayerInventory playerInventory;
    private BowAttackExecutor bowAttackExecutor;
    private MeleeAttackExecutor meleeAttackExecutor;

    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;
    [SerializeField] private float attackLockStart = 0.3f;
    [SerializeField] private float attackLockEnd = 0.6f;

    private float attackTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine attackCoroutine;
    private WeaponType currentAttackWeapon = WeaponType.None;

    private List<string> attackTriggers = new List<string>
    {
        "OneHandedMeleeAttack",
        "TwoHandedMeleeAttack",
        "BowAttack",
        "FistAttack",
        "Attack"
    };

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        character = GetComponent<Character>();
        canMoveState = GetComponent<CanMoveState>();
        playerInventory = GetComponent<PlayerInventory>();
        bowAttackExecutor = GetComponent<BowAttackExecutor>();
        meleeAttackExecutor = GetComponent<MeleeAttackExecutor>();
    }

    private void Update()
    {
        AttackTimerTick();
        AnimationTimerTick();
        canMoveState.isAttacking = isAttackLocked;
    }

    private void AnimationTimerTick()
    {
        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= attackLockStart && progress <= attackLockEnd)
                isAttackLocked = true;
            else if (isAttackLocked && progress > attackLockEnd)
                isAttackLocked = false;
        }
        else
        {
            isAttackLocked = false;
        }
    }

    private void AttackTimerTick()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private float GetAttackTime()
    {
        float attackTime = defaultTimeToAttack;
        attackTime /= character.GetStatsValue(Statistic.AttackSpeed).float_value;
        return attackTime;
    }

    public void ProcessCommand(Command command)
    {
        if (command == null || (command.target == null && command.commandType != CommandType.Attack))
            return;

        if (!CheckAttack())
            return;

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;
        bool isBow = weaponType == WeaponType.Bow;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            bowAttackExecutor?.ResetState();
            meleeAttackExecutor?.ResetState();
        }

        currentAttackWeapon = weaponType;

        if (isBow)
        {
            if (bowAttackExecutor != null)
            {
                bowAttackExecutor.HandleBowAttack(command, attackAnimationTime,
                    ResetAttackTimer, SetAnimationTimer, TriggerAttackAnimation, ref attackCoroutine);
            }
        }
        else
        {
            if (meleeAttackExecutor != null)
            {
                meleeAttackExecutor.HandleMeleeAttack(command, attackAnimationTime,
                    () => CheckAttack() && currentAttackWeapon == weaponType, ResetAttackTimer, SetAnimationTimer, TriggerAttackAnimation, ref attackCoroutine);
            }
        }
    }

    private string GetAttackTrigger()
    {
        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        foreach (string trigger in attackTriggers)
        {
            if (trigger == "FistAttack" && (weaponType == WeaponType.None || weapon == null) && AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                return trigger;
            if (trigger == "OneHandedMeleeAttack" && weaponType == WeaponType.OneHandedAxe && AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                return trigger;
            if (trigger == "TwoHandedMeleeAttack" && weaponType == WeaponType.TwoHandedAxe && AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                return trigger;
            if (trigger == "BowAttack" && weaponType == WeaponType.Bow && AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                return trigger;
            if (trigger == "Attack" && AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                return trigger;
        }
        return null;
    }

    private void TriggerAttackAnimation()
    {
        string attackTrigger = GetAttackTrigger();
        if (!string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }

    private bool AnimatorHasParameter(string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == paramName)
                return true;
        }
        return false;
    }

    private void SetAnimationTimer()
    {
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
    }

    public bool CheckAttack()
    {
        return attackTimer <= 0f;
    }

    public void ResetAttackTimer()
    {
        attackTimer = GetAttackTime();
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        bowAttackExecutor?.ResetState();
        meleeAttackExecutor?.ResetState();

        foreach (string trigger in attackTriggers)
        {
            if (AnimatorHasParameter(trigger, AnimatorControllerParameterType.Trigger))
                animator.ResetTrigger(trigger);
        }

        animationTimer = 0f;
        isAttackLocked = false;
        currentAttackWeapon = WeaponType.None;
    }

    public void ResetState()
    {
        CancelAttack();
    }
}
