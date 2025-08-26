using CharacterCommand;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CharacterDefeatHandler : MonoBehaviour
{
    public UnityEvent onDefeated;
    public UnityEvent onRespawned;

    public void Defeated()
    {
        onDefeated?.Invoke();
    }

    public void Respawn()
    {
        onRespawned?.Invoke();

        var commandHandler = GetComponent<CommandHandler>();
        if (commandHandler != null)
        {
            commandHandler.ClearCurrentCommand();
        }
    }
}
