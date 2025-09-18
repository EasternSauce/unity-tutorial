using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCharacterInput : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] MouseInput mouseInput;
    CommandHandler commandHandler;

    AttackInput attackInput;
    CursorTargetingHandler interactInput;

    bool isOverUIElement;
    bool isLMBPressed;
    bool isHoldActive = false;

    private bool commandLock = false;
    private float commandLockDuration = 0.3f;
    private float commandLockTimer = 0f;

    private CommandType currentCommandType = CommandType.None;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();
        attackInput = GetComponent<AttackInput>();
        interactInput = GetComponent<CursorTargetingHandler>();
    }

    private void Update()
    {
        isOverUIElement = EventSystem.current.IsPointerOverGameObject();

        if (commandLock)
        {
            commandLockTimer -= Time.deltaTime;
            if (commandLockTimer <= 0f)
                commandLock = false;
        }

        if (isHoldActive && !isOverUIElement && !commandLock)
            LMB_Hold_CommandProcess();
    }

    private void LMB_Hold_CommandProcess()
    {
        if (currentCommandType == CommandType.Interact)
            return;

        if (isLMBPressed && !isOverUIElement)
        {
            if (inventoryController.HasItemOnCursor && !inventoryController.SelectedItemController.SelectedItem.IsEquipped)
            {
                inventoryController.ThrowItemOnGround();
                SetCommandLock();
                return;
            }

            if (interactInput.TryGetTerrainPoint(out var point))
                MoveCommand(point);
        }
    }

    public void LMB_InputHandle(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            isLMBPressed = true;
            isHoldActive = true;

            if (!isOverUIElement)
                LMB_Press_ProcessCommand();
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
        if (inventoryController.HasItemOnCursor && !inventoryController.SelectedItemController.SelectedItem.IsEquipped)
        {
            inventoryController.ThrowItemOnGround();
            SetCommandLock();
            return;
        }

        if (interactInput.InteractCheck())
        {
            InteractCommand(interactInput.hoveringOverObject.gameObject);
            SetCommandLock();
            return;
        }

        if (interactInput.TryGetTerrainPoint(out var point))
            MoveCommand(point);
    }

    private void SetCommandLock()
    {
        commandLock = true;
        commandLockTimer = commandLockDuration;
    }

    private void MoveCommand(Vector3 point)
    {
        CancelOngoingAttack();
        currentCommandType = CommandType.Move;
        commandHandler.ExecuteCommand(new Command(CommandType.Move, point));
        currentCommandType = CommandType.None;
    }

    private void InteractCommand(GameObject target)
    {
        currentCommandType = CommandType.Interact;
        commandHandler.ExecuteCommand(new Command(CommandType.Interact, target));
        currentCommandType = CommandType.None;
    }

    private void CancelOngoingAttack()
    {
        var attackHandler = GetComponent<AttackHandler>();
        if (attackHandler != null)
            attackHandler.CancelAttack();
    }
}
