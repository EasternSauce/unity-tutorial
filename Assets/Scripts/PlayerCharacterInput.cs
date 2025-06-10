using CharacterCommand;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCharacterInput : MonoBehaviour
{
    [SerializeField] MouseInput mouseInput;
    CommandHandler commandHandler;

    AttackInput attackInput;
    InteractInput interactInput;

    bool isOverUIElement;
    bool isLMBPressed;

    bool isHoldActive = false;

    private bool commandLock = false;
    private float commandLockDuration = 0.3f;
    private float commandLockTimer = 0f;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();

        attackInput = GetComponent<AttackInput>();
        interactInput = GetComponent<InteractInput>();
    }

    private void Update()
    {
        isOverUIElement = EventSystem.current.IsPointerOverGameObject();

        if (commandLock)
        {
            commandLockTimer -= Time.deltaTime;
            if (commandLockTimer <= 0f)
            {
                commandLock = false;
            }
        }

        if (isHoldActive && !isOverUIElement && !commandLock)
        {
            LMB_Hold_CommandProcess();
        }
    }

    private void LMB_Hold_CommandProcess()
    {
        if (commandHandler.GetCurrentCommandType() == CommandType.Interact)
        {
            return;
        }

        if (isLMBPressed && isOverUIElement == false)
        {
            if (attackInput.AttackTargetCheck())
            {
                if (attackInput.AttackCooldownCheck())
                {
                    AttackCommand(interactInput.hoveringOverObject.gameObject);
                }
                return;
            }

            MoveCommand(mouseInput.rayToWorldIntersectionPoint);
        }
    }

    public void LMB_InputHandle(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            isLMBPressed = true;
            isHoldActive = true;

            if (!isOverUIElement)
            {
                LMB_Press_ProcessCommand();
            }
        }

        if (callbackContext.canceled)
        {
            isLMBPressed = false;
            isHoldActive = false;
            commandLock = false;
        }
    }

    private void LMB_Press_ProcessCommand()
    {
        if (attackInput.AttackTargetCheck() && attackInput.AttackCooldownCheck())
        {
            AttackCommand(interactInput.hoveringOverObject.gameObject);
            SetCommandLock();
            return;
        }

        if (interactInput.InteractCheck())
        {
            InteractCommand(interactInput.hoveringOverObject.gameObject);
            SetCommandLock();
            return;
        }

        MoveCommand(mouseInput.rayToWorldIntersectionPoint);
    }

    private void SetCommandLock()
    {
        commandLock = true;
        commandLockTimer = commandLockDuration;
    }

    private void MoveCommand(Vector3 point)
    {
        CancelOngoingAttack();
        commandHandler.SetCommand(new Command(CommandType.Move, point));
    }

    private void InteractCommand(GameObject target)
    {
        commandHandler.SetCommand(new Command(CommandType.Interact, target));
    }

    private void AttackCommand(GameObject target)
    {
        commandHandler.SetCommand(new Command(CommandType.Attack, target));
    }

    private void CancelOngoingAttack()
    {
        var attackHandler = GetComponent<AttackHandler>();
        if (attackHandler != null)
        {
            attackHandler.ResetState();
        }
    }
}
