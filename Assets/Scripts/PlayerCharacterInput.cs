using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCharacterInput : MonoBehaviour
{
    [SerializeField] MouseInput mouseInput;

    CharacterMovementInput characterMovementInput;
    AttackInput attackInput;
    InteractInput interactInput;

    private void Awake()
    {
        characterMovementInput = GetComponent<CharacterMovementInput>();
        attackInput = GetComponent<AttackInput>();
        interactInput = GetComponent<InteractInput>();
    }

    public void LMB_InputHandle(InputAction.CallbackContext callbackContext)
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (attackInput.AttackCheck())
        {
            attackInput.Attack();
            return;
        }

        if (interactInput.InteractCheck())
        {
            interactInput.Interact();
            return;
        }

        interactInput.ResetState();
        characterMovementInput.MoveInput();
    }
}
