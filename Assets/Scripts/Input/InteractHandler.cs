using CharacterCommand;
using UnityEngine;

public class InteractHandler : MonoBehaviour, ICommandHandle
{
    [SerializeField] float interactRange = 0.5f;

    MoveHandler characterMovement;
    Character character;

    private void Awake()
    {
        characterMovement = GetComponent<MoveHandler>();
        character = GetComponent<Character>();
    }

    public void ProcessCommand(Command command)
    {
        float distance = Vector3.Distance(transform.position, command.target.transform.position);

        if (distance < interactRange)
        {
            command.target.GetComponent<InteractableObject>().Interact(character);
            characterMovement.Stop();
            command.isComplete = true;
        }
        else
        {
            characterMovement.SetDestination(command.target.transform.position);
        }
    }

    public void CancelCommand()
    {

    }
}
