using UnityEngine;

public class InteractCommandHandler : MonoBehaviour, ICommandHandler
{
    private float interactRange = 2f;

    private MoveCommandHandler characterMovement;
    private Character character;
    private Command currentCommand;

    private void Awake()
    {
        characterMovement = GetComponent<MoveCommandHandler>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (currentCommand == null)
        {
            return;
        }

        if (currentCommand.target == null)
        {
            currentCommand.isComplete = true;
            currentCommand = null;
            return;
        }

        float distance = DistanceHelper.Distance(transform.position, currentCommand.target.transform.position);

        if (distance <= interactRange)
        {
            InteractableObject io = currentCommand.target.GetComponent<InteractableObject>();
            if (io != null)
            {
                io.Interact(character);
            }

            characterMovement.Stop();
            currentCommand.isComplete = true;
            currentCommand = null;
        }
        else
        {
            characterMovement.MoveTo(currentCommand.target.transform.position);
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
