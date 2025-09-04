using CharacterCommand;
using UnityEngine;
using UnityEngine.Events;

public class CharacterDefeatHandler : MonoBehaviour
{
    public UnityEvent onDefeated;
    public UnityEvent onRespawned;

    public bool IsDefeated { get; private set; }

    public void Defeated()
    {
        IsDefeated = true;
        onDefeated?.Invoke();
    }

    public void Respawn()
    {
        IsDefeated = false;
        onRespawned?.Invoke();

        var commandHandler = GetComponent<CommandHandler>();
        if (commandHandler != null)
        {
            commandHandler.ClearCurrentCommand();
        }
    }
}
