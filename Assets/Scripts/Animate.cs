using UnityEngine;
using UnityEngine.AI;

public class Animate : MonoBehaviour
{
    Animator animator;

    NavMeshAgent agent;

    Character character;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        float motion = agent.velocity.magnitude;

        animator.SetFloat("motion", motion);
        animator.SetBool("defeated", character.IsDead);
    }
}
