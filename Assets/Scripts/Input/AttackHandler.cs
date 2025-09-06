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

    private float attackTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private Coroutine attackCoroutine;
    private WeaponType currentAttackWeapon = WeaponType.None;
    private GameObject currentTarget;
    private Command queuedCommand;

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
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= 0.3f && progress <= 0.6f) isAttackLocked = true;
            else if (isAttackLocked && progress > 0.6f) isAttackLocked = false;
        }
        else
        {
            isAttackLocked = false;
        }
        canMoveState.isAttacking = isAttackLocked;
    }

    private float GetAttackTime()
    {
        float attackTime = defaultTimeToAttack;
        attackTime /= character.GetStatsValue(Statistic.AttackSpeed).float_value;
        return attackTime;
    }

    public void ProcessCommand(Command command)
    {
        if (command == null || command.commandType != CommandType.Attack) return;

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        currentAttackWeapon = weaponType;

        if (CheckAttack())
        {
            queuedCommand = null;
            StartAttack(command, weaponType);
        }
        else
        {
            queuedCommand = command;
        }
    }


    private void StartAttack(Command command, WeaponType weaponType)
    {
        if (currentTarget != null && currentTarget != command.target)
            CancelAttack();

        currentTarget = command.target;

        ResetAttackTimer(); // start cooldown immediately

        if (weaponType == WeaponType.Bow)
        {
            bowAttackExecutor?.HandleBowAttack(command, attackAnimationTime,
                SetAnimationTimer, TriggerAttackAnimation, ref attackCoroutine, OnAttackFinished);
        }
        else
        {
            meleeAttackExecutor?.HandleMeleeAttack(command, attackAnimationTime,
                () => CheckAttack(),
                ResetAttackTimer, SetAnimationTimer, TriggerAttackAnimation, ref attackCoroutine, OnAttackFinished);
        }
    }


    private string GetAttackTrigger()
    {
        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        foreach (string trigger in attackTriggers)
        {
            if (trigger == "FistAttack" && (type == WeaponType.None || weapon == null) && AnimatorHasParameter(trigger))
                return trigger;
            if (trigger == "OneHandedMeleeAttack" && type == WeaponType.OneHandedAxe && AnimatorHasParameter(trigger))
                return trigger;
            if (trigger == "TwoHandedMeleeAttack" && type == WeaponType.TwoHandedAxe && AnimatorHasParameter(trigger))
                return trigger;
            if (trigger == "BowAttack" && type == WeaponType.Bow && AnimatorHasParameter(trigger))
                return trigger;
            if (trigger == "Attack" && AnimatorHasParameter(trigger))
                return trigger;
        }
        return null;
    }

    private void TriggerAttackAnimation()
    {
        string trigger = GetAttackTrigger();
        if (string.IsNullOrEmpty(trigger)) return;
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);
        animator.SetTrigger(trigger);
    }

    private bool AnimatorHasParameter(string paramName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == UnityEngine.AnimatorControllerParameterType.Trigger && param.name == paramName)
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
            if (AnimatorHasParameter(trigger))
                animator.ResetTrigger(trigger);
        }

        animationTimer = 0f;
        isAttackLocked = false;
        currentAttackWeapon = WeaponType.None;
        currentTarget = null;
        queuedCommand = null;
    }

    private void OnAttackFinished()
    {
        attackCoroutine = null;
        if (queuedCommand != null)
        {
            Command commandToAttack = queuedCommand;
            queuedCommand = null;
            StartAttack(commandToAttack, currentAttackWeapon);
        }
    }

    public void ResetState()
    {
        CancelAttack();
    }
}
