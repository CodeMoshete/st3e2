using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavNodeAttribute
{
    public string Name;
    public bool Value;
}

public class NavNode : MonoBehaviour
{
    public List<NavNodeLink> Links;
    public float TriggerRadius = 1f;
    public string ExitNodeTag;
    public CustomAction ArrivalAction;
    public bool CaptureCharacterForAction;
    [HideInInspector]
    public CharacterEntity CurrentCharacter; // IF USED - MUST BE MANUALLY RELEASED!
    public List<NavNodeAttribute> Attributes = new List<NavNodeAttribute>();

    public void Initialize()
    {
        for (int i = 0, count = Links.Count; i < count; ++i)
        {
            Links[i].SourceNode = this;
        }
    }

    public NavNodeAttribute GetAttribute(string attributeName)
    {
        NavNodeAttribute retAtr = null;
        for (int i = 0, count = Attributes.Count; i < count; ++i)
        {
            if (Attributes[i].Name == attributeName)
            {
                retAtr = Attributes[i];
                break;
            }
        }
        return retAtr;
    }

    public void SetAttribute(string attributeName, bool value)
    {
        NavNodeAttribute attribute = GetAttribute(attributeName);
        if (attribute == null)
        {
            attribute = new NavNodeAttribute();
            Attributes.Add(attribute);
        }

        attribute.Name = attributeName;
        attribute.Value = value;
    }

    public void RemoveAttribute(string attributeName)
    {
        NavNodeAttribute attributeToRemove = null;
        for (int i = 0, count = Attributes.Count; i < count; ++i)
        {
            if (Attributes[i].Name == attributeName)
            {
                attributeToRemove = Attributes[i];
                break;
            }
        }

        if (attributeToRemove != null)
        {
            Attributes.Remove(attributeToRemove);
        }
    }
}
