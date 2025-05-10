using UnityEngine;

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

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
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
}
