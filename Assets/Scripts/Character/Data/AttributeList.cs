using System;
using System.Collections.Generic;

[Serializable]
public class AttributeList
{
    public List<AttributeValue> attributeValues;

    public AttributeList()
    {
        attributeValues = new List<AttributeValue>();
    }

    public void Init()
    {
        attributeValues.Add(new AttributeValue(Attribute.Strength));
        attributeValues.Add(new AttributeValue(Attribute.Dexterity));
        attributeValues.Add(new AttributeValue(Attribute.Intelligence));
    }

    public AttributeValue Get(Attribute attributeToShow)
    {
        return attributeValues[(int)attributeToShow];
    }
}
