using UnityEngine;
using UnityEngine.InputSystem;
using CharacterCommand;

public class AttackInput : MonoBehaviour
{
    InteractInput interactInput;
    CommandHandler commandHandler;
    MouseInput mouseInput;

    void Awake()
    {
        interactInput = GetComponent<InteractInput>();
        commandHandler = GetComponent<CommandHandler>();
        mouseInput = Object.FindFirstObjectByType<MouseInput>();
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
                commandHandler.SetCommand(bowAttackCommand);
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
                    commandHandler.SetCommand(meleeAttackCommand);
                }
            }
        }
    }

    public void OnLMB(InputAction.CallbackContext ctx)
    {
    }

    public bool AttackTargetCheck()
    {
        return interactInput.attackTarget != null;
    }
}
