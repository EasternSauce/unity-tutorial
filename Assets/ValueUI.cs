using TMPro;
using UnityEngine;

public class ValueUI : MonoBehaviour
{
    public enum ValueUIShow
    {
        Attribute,
        Stats
    }

    public ValueUIShow showValueType;
    public Attribute attributeToShow;
    public Statistic statisticToShow;

    [SerializeField] TextMeshProUGUI text;

    public void ShowCharacterValue(Character character)
    {
        switch (showValueType)
        {
            case ValueUIShow.Attribute:
                AttributeValue attributeValue = character.GetAttributeValue(attributeToShow);
                SetText(attributeValue.value);
                break;
            case ValueUIShow.Stats:
                StatsValue statsValue = character.GetStatsValue(statisticToShow);
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
