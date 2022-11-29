using System.Collections.Generic;
using UnityEngine;

public class CharacterDirectiveSystem : ICharacterSystem
{
    private List<CharacterEntity> managedCharacters;

    public void Initialize()
    {
        managedCharacters = new List<CharacterEntity>();
    }

    public void AddCharacter(CharacterEntity character)
    {
        if (character.NavComponent == null)
        {
            character.DirectiveComponent = new CharacterDirectiveComponent();
            managedCharacters.Add(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} already has a CharacterDirectiveComponent!", character.name));
        }
    }

    public void RemoveCharacter(CharacterEntity character)
    {
        if (character.NavComponent != null)
        {
            character.NavComponent = null;
            managedCharacters.Remove(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} already has a CharacterDirectiveComponent!", character.name));
        }
    }

    public void Update(float dt)
    {
        for (int i = 0, count = managedCharacters.Count; i < count; ++i)
        {
            CharacterEntity character = managedCharacters[i];
        }
    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
