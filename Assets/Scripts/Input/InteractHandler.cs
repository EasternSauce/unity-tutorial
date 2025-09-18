using UnityEngine;

public class InteractHandler : MonoBehaviour, ICommandHandle
{
    [SerializeField] private float interactRange = 0.5f;

    private MoveHandler characterMovement;
    private Character character;
    private Command currentCommand;

    private void Awake()
    {
        characterMovement = GetComponent<MoveHandler>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (currentCommand == null) return;
        if (currentCommand.target == null)
        {
            currentCommand.isComplete = true;
            currentCommand = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentCommand.target.transform.position);

        if (distance <= interactRange)
        {
            var interactable = currentCommand.target.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Interact(character);
            }

            characterMovement.Stop();
            currentCommand.isComplete = true;
            currentCommand = null;
        }
        else
        {
            characterMovement.SetDestination(currentCommand.target.transform.position);
        }
    }

    public void ProcessCommand(Command command)
    {
        currentCommand = command;
    }

    public void CancelInteract()
    {
        if (currentCommand != null)
        {
            currentCommand.isComplete = true;
            currentCommand = null;
        }
        characterMovement.Stop();
    }
}
