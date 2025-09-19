using TMPro;
using UnityEngine;

public class CharacterStatText : MonoBehaviour
{
    public enum StatCategory
    {
        Attribute,
        Regular
    }

    public StatCategory statCategory;
    public Attribute attributeToShow;
    public RegularStat statisticToShow;

    [SerializeField] TextMeshProUGUI text;

    public void ShowCharacterValue(Character character)
    {
        switch (statCategory)
        {
            case StatCategory.Attribute:
                AttributeValue attributeValue = character.GetAttributeValue(attributeToShow);
                SetText(attributeValue.value);
                break;
            case StatCategory.Regular:
                RegularStatValue statsValue = character.GetStatsValue(statisticToShow);
                if (statsValue.typeFloat == true)
                {
                    SetText(statsValue.float_value);
                }
                else
                {
                    SetText(statsValue.integer_value);
                }
                break;
        }
    }

    public void SetText(float floatValue)
    {
        text.text = floatValue.ToString();
    }

    public void SetText(int integerValue)
    {
        text.text = integerValue.ToString();
    }
}
