using System.Collections.Generic;
using UnityEngine;

public class CharacterConversationSystem : ICharacterSystem
{
    private const float CONVERSATION_SQR_DST = 2.4f;
    private List<CharacterEntity> managedCharacters;
    private Transform player;

    public void Initialize()
    {
        managedCharacters = new List<CharacterEntity>();
        player = GameObject.Find("OVRPlayerController").transform;
    }

    public void AddCharacter(CharacterEntity character)
    {
        if (!managedCharacters.Contains(character))
        {
            managedCharacters.Add(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} has already been added!", character.name));
        }
    }

    public void RemoveCharacter(CharacterEntity character)
    {
        if (managedCharacters.Contains(character))
        {
            managedCharacters.Remove(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} is not managed!", character.name));
        }
    }

    public void Update(float dt)
    {
        for (int i = 0, count = managedCharacters.Count; i < count; ++i)
        {
            CharacterEntity character = managedCharacters[i];
            if (character.IsViewVisible)
            {
                float sqrDist = Vector3.SqrMagnitude(character.transform.position - player.position);
                if (!character.ConversationComponent.IsInRange && sqrDist <= CONVERSATION_SQR_DST)
                {
                    Debug.Log("In range: " + character.name);
                    character.ConversationComponent.IsInRange = true;
                }
                else if (character.ConversationComponent.IsInRange && sqrDist > CONVERSATION_SQR_DST)
                {
                    Debug.Log("Out of range: " + character.name);
                    character.ConversationComponent.IsInRange = false;
                }
            }
        }
    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
