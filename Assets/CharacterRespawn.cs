using UnityEngine;

public class CharacterRespawn : MonoBehaviour
{
    Vector3 respawnPoint;
    string respawnSceneName;
    CharacterDefeatHandler characterDefeat;

    private void Start()
    {
        respawnPoint = transform.position;
    }

    public void Respawn()
    {
        gameObject.transform.position = respawnPoint;
        characterDefeat.Respawn();
    }
}
