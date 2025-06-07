using UnityEngine;

public class CharacterRespawn : MonoBehaviour
{
    Vector3 respawnPoint;
    string respawnSceneName;
    CharacterDefeatHandler characterDefeat;
    [SerializeField] Animator animator;

    private void Awake()
    {
        characterDefeat = GetComponent<CharacterDefeatHandler>();
    }

    private void Start()
    {
        respawnPoint = transform.position;
    }

    public void Respawn()
    {
        gameObject.transform.position = respawnPoint;
        characterDefeat.Respawn();

        AttackHandler attackHandler = GetComponent<AttackHandler>();
        if (attackHandler != null)
        {
            attackHandler.ResetState();
        }

        animator.Play("Idle");
        animator.SetBool("defeated", false);
    }
}
