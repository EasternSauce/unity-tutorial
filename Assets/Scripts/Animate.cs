using UnityEngine;
using UnityEngine.AI;

public class Animate : MonoBehaviour
{
    Animator animator;

    NavMeshAgent agent;

    private void Awake() 
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private void Update()
    {
        float motion = agent.velocity.magnitude;

        animator.SetFloat("motion", motion);
    }
}
