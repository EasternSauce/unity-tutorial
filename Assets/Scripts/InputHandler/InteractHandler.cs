using CharacterCommand;
using UnityEngine;

public class InteractHandler : MonoBehaviour, ICommandHandle
{
    [SerializeField] float interactRange = 0.5f;

    CharacterMovement characterMovement;
    Inventory inventory;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        inventory = GetComponent<Inventory>();
    }

    public void ProcessCommand(Command command)
    {
        Debug.Log("Processing command");
        float distance = Vector3.Distance(transform.position, command.target.transform.position);

        if (distance < interactRange)
        {
            Debug.Log("Within range, interacting");
            var interactable = command.target.GetComponent<InteractableObject>();
            if (interactable == null)
            {
                Debug.LogError("No InteractableObject on command.target!");
            }
            else
            {
                interactable.Interact(inventory);
            }
            characterMovement.Stop();
            command.isComplete = true;
        }
        else
        {
            characterMovement.SetDestination(command.target.transform.position);
        }
    }
}
