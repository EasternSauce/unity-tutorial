using UnityEngine;
using UnityEngine.AI;

public class CharacterRespawn : MonoBehaviour
{
    Vector3? respawnPoint;
    CharacterDefeatHandler characterDefeat;
    [SerializeField] Animator animator;

    private void Awake()
    {
        characterDefeat = GetComponent<CharacterDefeatHandler>();
    }

    private void Start()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            respawnPoint = hit.position;
        }
        else
        {
            respawnPoint = null;
        }
    }

    public void Respawn()
    {
        if (respawnPoint == null)
            return;

        transform.position = respawnPoint.Value;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (!agent.enabled)
                agent.enabled = true;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                agent.enabled = false;
            }
        }

        characterDefeat.Respawn();

        AttackHandler attackHandler = GetComponent<AttackHandler>();
        if (attackHandler != null)
            attackHandler.ResetState();

        if (animator != null)
        {
            animator.Play("Idle");
            animator.SetBool("defeated", false);
        }
    }
}