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
    public bool CaptureCharactersForAction;
    public bool DisableNavigationOnArrival;

    [HideInInspector]
    public string ParentNetworkName;

    [HideInInspector]
    // IF USED - MUST BE MANUALLY RELEASED!
    public CharacterEntity CurrentCharacter
    {
        get
        {
            if (CharactersInQueue.Count > 0)
            {
                return CharactersInQueue[0];
            }
            return null;
        }
    }

    [HideInInspector]
    public List<CharacterEntity> CharactersInQueue = new List<CharacterEntity>();
    public List<NavNodeAttribute> Attributes = new List<NavNodeAttribute>();

    public void Initialize(string parentNetworkName)
    {
        ParentNetworkName = parentNetworkName;
        for (int i = 0, count = Links.Count; i < count; ++i)
        {
            Links[i].SourceNode = this;
        }
    }

    public void EnqueueCharacter(CharacterEntity character)
    {
        Debug.Log("Character enqueued to " + name + ": " + character.name);
        CharactersInQueue.Add(character);
    }

    public void ReleaseCurrentCharacter()
    {
        if (CharactersInQueue.Count > 0)
        {
            Debug.Log("Character dequeued from " + name + ": " + CharactersInQueue[0].name);
            CharactersInQueue.RemoveAt(0);

            if (CharactersInQueue.Count > 0 && ArrivalAction != null)
            {
                ArrivalAction.Initiate();
            }
        }
        else
        {
            Debug.LogError("Attempted to release a character, but the queue is empty!");
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
