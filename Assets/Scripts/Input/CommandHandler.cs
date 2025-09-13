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
                GetComponent<MoveHandler>()?.ProcessCommand(command);
                break;
            case CommandType.Attack:
                GetComponent<AttackHandler>()?.ProcessCommand(command);
                break;
            case CommandType.Interact:
                GetComponent<InteractHandler>()?.ProcessCommand(command);
                break;
        }
    }

    public void CancelCurrentCommand()
    {
        GetComponent<MoveHandler>()?.Stop();
        GetComponent<AttackHandler>()?.CancelAttack();
    }
}
