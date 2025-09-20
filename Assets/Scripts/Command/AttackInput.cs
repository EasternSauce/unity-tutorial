using UnityEngine;
using UnityEngine.InputSystem;

public class AttackInput : MonoBehaviour
{
    PlayerCursorTargetingHandler interactInput;
    CommandHandler commandHandler;
    PlayerMouseInput mouseInput;

    void Awake()
    {
        interactInput = GetComponent<PlayerCursorTargetingHandler>();
        commandHandler = GetComponent<CommandHandler>();
        mouseInput = Object.FindFirstObjectByType<PlayerMouseInput>();
    }

    public void OnRMB(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        InventoryItem weapon = playerInventory?.CurrentWeapon;

        if (weapon != null && weapon.itemData.weaponType == WeaponType.Bow)
        {
            if (mouseInput != null)
            {
                Vector3 clickPos = mouseInput.rayToWorldIntersectionPoint;
                Command bowAttackCommand = new Command(CommandType.Attack, clickPos);
                commandHandler.ExecuteCommand(bowAttackCommand);
            }
        }
        else
        {
            IDamageable target = interactInput.attackTarget;

            if (target != null)
            {
                MonoBehaviour mb = target as MonoBehaviour;
                if (mb != null)
                {
                    Command meleeAttackCommand = new Command(CommandType.Attack, mb.gameObject);
                    commandHandler.ExecuteCommand(meleeAttackCommand);
                }
            }
            else
            {
                Command whiffAttackCommand = new Command(CommandType.Attack, (GameObject)null);
                commandHandler.ExecuteCommand(whiffAttackCommand);
            }
        }
    }

    public bool AttackTargetCheck()
    {
        return interactInput.attackTarget != null;
    }
}
