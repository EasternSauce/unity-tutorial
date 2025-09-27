using System.Collections.Generic;
using UnityEngine;

public class CombatActionController : MonoBehaviour
{
    private Dictionary<CombatActionType, CombatActionExecutor> executors = new();

    private void Awake()
    {
        Character character = GetComponent<Character>();
        MoveCommandHandler movement = GetComponent<MoveCommandHandler>();
        Animator anim = GetComponentInChildren<Animator>();

        if (character == null) Debug.LogError("[CombatActionController] No Character component found!");
        if (movement == null) Debug.LogError("[CombatActionController] No MoveCommandHandler component found!");
        if (anim == null) Debug.LogError("[CombatActionController] No Animator found in children!");

        RegisterExecutor(CombatActionType.Melee, new MeleeAttackExecutor(character, movement, anim));
    }

    private void Update()
    {
        foreach (var executor in executors.Values)
            executor.TickUpdate();
    }

    private void RegisterExecutor(CombatActionType type, CombatActionExecutor executor)
    {
        if (!executors.ContainsKey(type))
            executors[type] = executor;
    }

    public void Execute(CombatActionType type, Command command)
    {
        if (executors.TryGetValue(type, out var executor))
        {
            executor.Execute(command);
        }
    }

    public void ResetAllExecutors()
    {
        foreach (var executor in executors.Values)
            executor.ResetState();
    }

    public T GetExecutor<T>(CombatActionType type) where T : CombatActionExecutor
    {
        if (executors.TryGetValue(type, out var executor) && executor is T typedExecutor)
            return typedExecutor;
        return null;
    }
}
