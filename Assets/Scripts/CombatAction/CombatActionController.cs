using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character), typeof(MoveCommandHandler))]
public class CombatActionController : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject fireballPrefab;

    private Dictionary<CombatActionType, CombatActionExecutor> executors = new();

    private void Awake()
    {
        Character character = GetComponent<Character>();
        MoveCommandHandler movement = GetComponent<MoveCommandHandler>();
        Animator anim = GetComponentInChildren<Animator>();

        if (character == null) Debug.LogError("[CombatActionController] No Character component found!");
        if (movement == null) Debug.LogError("[CombatActionController] No MoveCommandHandler component found!");
        if (anim == null) Debug.LogError("[CombatActionController] No Animator found in children!");

        // Register Melee Executor
        RegisterExecutor(CombatActionType.Melee, new MeleeAttackExecutor(character, movement, anim));

        // Register Bow Executor
        if (arrowPrefab != null)
        {
            RegisterExecutor(CombatActionType.Bow, new BowAttackExecutor(character, movement, anim, arrowPrefab));
        }
        else
        {
            Debug.LogWarning("[CombatActionController] Arrow prefab not assigned! Bow attacks will not spawn projectiles.");
        }

        // Register Fireball Executor
        if (fireballPrefab != null)
        {
            RegisterExecutor(CombatActionType.Fireball, new FireballAbilityExecutor(character, movement, anim, fireballPrefab));
        }
        else
        {
            Debug.LogWarning("[CombatActionController] Fireball prefab not assigned! Fireball attacks will not spawn projectiles.");
        }
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
