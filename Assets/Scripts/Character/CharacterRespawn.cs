using CharacterCommand;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterDefeatHandler))]
public class CharacterRespawn : MonoBehaviour
{
    private Vector3? respawnPoint;
    private CharacterDefeatHandler characterDefeat;
    [SerializeField] private Animator animator;

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
            attackHandler.CancelAttack();

        if (animator != null)
        {
            animator.Play("Idle");
            animator.SetBool("defeated", false);
        }
    }
}
