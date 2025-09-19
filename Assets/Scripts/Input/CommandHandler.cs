using UnityEngine;

public class CommandHandler : MonoBehaviour
{
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void ExecuteCommand(Command command)
    {
        if (command == null) return;
        if (command.target != null && command.target.GetComponent<Character>()?.IsDead == true) return;

        switch (command.commandType)
        {
            case CommandType.Move:
                CancelMoveAndInteract();
                GetComponent<MoveHandler>()?.ProcessCommand(command);
                break;

            case CommandType.Interact:
                CancelMoveAndInteract();
                GetComponent<InteractHandler>()?.ProcessCommand(command);
                break;

            case CommandType.Attack:
                GetComponent<AttackHandler>()?.ProcessCommand(command);
                break;
        }
    }

    private void CancelMoveAndInteract()
    {
        GetComponent<MoveHandler>()?.Stop();
        GetComponent<InteractHandler>()?.CancelInteract();
    }

    public void CancelCurrentCommand()
    {
        CancelMoveAndInteract();
        GetComponent<AttackHandler>()?.CancelAttack();
    }
}
