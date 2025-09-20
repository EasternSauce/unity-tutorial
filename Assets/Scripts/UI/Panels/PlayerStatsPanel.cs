using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsPanel : MonoBehaviour
{
    [SerializeField] List<PlayerStatText> attributeValueUIElements;
    [SerializeField] List<PlayerStatText> statsValueUIElements;
    [SerializeField] Character targetCharacter;

    private void Update()
    {
        UpdatePanel(targetCharacter);
    }

    public void UpdatePanel(Character character)
    {
        for (int i = 0; i < attributeValueUIElements.Count; i++)
        {
            attributeValueUIElements[i].ShowCharacterValue(character);
        }
        for (int i = 0; i < statsValueUIElements.Count; i++)
        {
            statsValueUIElements[i].ShowCharacterValue(character);
        }
    }
}
