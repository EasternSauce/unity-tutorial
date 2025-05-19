using UnityEngine;

public class CharacterMovementInput : MonoBehaviour
{
    [SerializeField] MouseInput mouseInput;
    CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    public void MoveInput()
    {
        characterMovement.SetDestination(mouseInput.rayToWorldIntersectionPoint);
    }
}
