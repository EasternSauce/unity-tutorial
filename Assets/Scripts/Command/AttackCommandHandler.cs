using UnityEngine;

[RequireComponent(typeof(Character))]
public class AttackCommandHandler : MonoBehaviour, ICommandHandler
{
    private PlayerInventory playerInventory;
    private BowAttackExecutor bowAttackExecutor;
    private MeleeAttackExecutor meleeAttackExecutor;
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
        playerInventory = GetComponent<PlayerInventory>();
        bowAttackExecutor = GetComponent<BowAttackExecutor>();
        meleeAttackExecutor = GetComponent<MeleeAttackExecutor>();
    }

    public void ProcessCommand(Command command)
    {
        if (command == null || command.commandType != CommandType.Attack) return;

        if (command.target != null && command.target.GetComponent<Character>()?.IsDead == true) return;

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        character.isPerformingCombatAction = true;

        if (weaponType == WeaponType.Bow)
            bowAttackExecutor?.HandleBowAttack(command);
        else
            meleeAttackExecutor?.HandleMeleeAttack(command);
    }

    public void CancelAttack()
    {
        bowAttackExecutor?.ResetState();
        meleeAttackExecutor?.CancelCurrentAttack();

        if (character != null)
            character.isPerformingCombatAction = false;
    }
}
