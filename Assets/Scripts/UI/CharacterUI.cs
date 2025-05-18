using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    [SerializeField] UIPoolBar hpBar;
    [SerializeField] UIPoolBar energyBar;

    private void Update()
    {
        hpBar.Show(character.lifePool);
        energyBar.Show(character.energyPool);
    }
}
