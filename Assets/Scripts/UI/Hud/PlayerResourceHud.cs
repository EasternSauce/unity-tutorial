using UnityEngine;

public class PlayerResourceHud : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    [SerializeField] ResourceBar hpBar;
    [SerializeField] ResourceBar energyBar;

    private void Update()
    {
        hpBar.Show(character.lifePool);
        energyBar.Show(character.energyPool);
    }
}
