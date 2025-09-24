using UnityEngine;

[RequireComponent(typeof(Character))]
public class AttackCommandHandler : MonoBehaviour, ICommandHandler
{
    private PlayerInventory playerInventory;
    private Character character;
    private CombatActionController combatActionController;

    private void Awake()
    {
        character = GetComponent<Character>();
        playerInventory = GetComponent<PlayerInventory>();
        combatActionController = GetComponent<CombatActionController>();
    }

    public void ProcessCommand(Command command)
    {
        if (command == null)
        {
            return;
        }

        if (command.commandType != CommandType.CombatAction)
        {
            return;
        }

        if (command.target != null && command.target.GetComponent<Character>()?.IsDead == true)
        {
            return;
        }

        InventoryItem weapon = playerInventory?.CurrentWeapon;
        WeaponType weaponType = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        if (weaponType == WeaponType.Bow)
        {
            combatActionController.Execute(CombatActionType.Bow, command);
        }
        else
        {
            combatActionController.Execute(CombatActionType.Melee, command);
        }
    }

    public void CancelAttack()
    {
        combatActionController.ResetAllExecutors();

        if (character != null)
            character.isPerformingCombatAction = false;
    }
}
