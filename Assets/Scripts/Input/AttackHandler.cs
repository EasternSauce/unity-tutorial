using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CanMoveState))]
public class AttackHandler : MonoBehaviour, ICommandHandle
{
    private PlayerInventory playerInventory;
    private BowAttackExecutor bowAttackExecutor;
    private MeleeAttackExecutor meleeAttackExecutor;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        bowAttackExecutor = GetComponent<BowAttackExecutor>();
        meleeAttackExecutor = GetComponent<MeleeAttackExecutor>();
    }

    public void ProcessCommand(Command command)
    {
        if (command == null || command.commandType != CommandType.Attack) return;

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        if (weaponType == WeaponType.Bow)
            bowAttackExecutor?.HandleBowAttack(command);
        else
            meleeAttackExecutor?.HandleMeleeAttack(command);
    }

    public void CancelAttack()
    {
        bowAttackExecutor?.ResetState();
        meleeAttackExecutor?.ResetState();
    }

}
