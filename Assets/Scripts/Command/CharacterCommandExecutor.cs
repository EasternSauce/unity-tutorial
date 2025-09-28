using UnityEngine;

public class CharacterCommandExecutor : MonoBehaviour
{
    public void ExecuteCommand(Command command)
    {
        if (command == null) return;
        if (command.target != null && command.target.GetComponent<Character>()?.IsDead == true) return;

        switch (command.commandType)
        {
            case CommandType.Move:
                CancelMoveAndInteract();
                GetComponent<CombatActionCommandHandler>()?.CancelAttack();
                GetComponent<MoveCommandHandler>()?.ProcessCommand(command);
                break;

            case CommandType.Interact:
                CancelMoveAndInteract();
                GetComponent<CombatActionCommandHandler>()?.CancelAttack();
                GetComponent<InteractCommandHandler>()?.ProcessCommand(command);
                break;

            case CommandType.CombatAction:
                GetComponent<CombatActionCommandHandler>()?.ProcessCommand(command);
                break;
        }
    }

    private void CancelMoveAndInteract()
    {
        GetComponent<MoveCommandHandler>()?.Stop();
        GetComponent<InteractCommandHandler>()?.CancelInteract();
    }

    public void CancelCurrentCommand()
    {
        CancelMoveAndInteract();
        GetComponent<CombatActionCommandHandler>()?.CancelAttack();
    }
}
