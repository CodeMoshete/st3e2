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
        if (!managedCharacters.Contains(character))
        {
            managedCharacters.Add(character);

            List<CharacterDirectiveData> worldDirectives = character.DirectiveComponent.WorldDirectives;
            NavWorldID currentWorld = Service.NavWorldManager.CurrentNavWorld.WorldID;
            character.DirectiveComponent.CurrentDirectiveData = null;
            for (int i = 0, count = worldDirectives.Count; i < count; ++i)
            {
                if (worldDirectives[i].NavWorld == currentWorld)
                {
                    character.DirectiveComponent.CurrentDirectiveData = worldDirectives[i];
                }
            }
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
            CharacterDirectiveComponent directiveComp = character.DirectiveComponent;

            if (directiveComp.CurrentDirective != null && 
                character.NavComponent.CurrentNode.name == directiveComp.CurrentDirective.NavNodeName &&
                character.NavComponent.FinalDestination == null)
            {
                directiveComp.CurrentDirectiveDuration -= dt;
                if (directiveComp.CurrentDirectiveDuration <= 0f)
                {
                    directiveComp.CurrentDirective = null;
                }
            }
            else
            {
                ChooseNewDirective(character, directiveComp);
            }
        }
    }

    private void ChooseNewDirective(CharacterEntity character, CharacterDirectiveComponent directiveComp)
    {

    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
