using System.Collections.Generic;
using UnityEngine;

public class CharacterDirectiveSystem : ICharacterSystem
{
    private const float DEFAULT_DIRECTIVE_TIMEOUT = 60f;
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

            if (character.NavComponent.IsInTransit)
            {
                continue;
            }

            CharacterDirectiveComponent directiveComp = character.DirectiveComponent;

            if (directiveComp.CurrentDirective != null && 
                !directiveComp.IsDirectiveExpired &&
                character.NavComponent.CurrentNode.name == directiveComp.CurrentDirective.NavNodeName &&
                character.NavComponent.FinalDestination == null)
            {
                directiveComp.CurrentDirectiveDuration -= dt;
                if (directiveComp.CurrentDirectiveDuration <= 0f)
                {
                    directiveComp.IsDirectiveExpired = true;
                }
            }
            else
            {
                // If there are no valid directives, then this character is waiting.
                if (directiveComp.CurrentDirectiveDuration > 0f)
                {
                    directiveComp.CurrentDirectiveDuration -= dt;
                    continue;
                }

                // Pick and pursue a new directive.
                ChooseNewDirective(character, directiveComp);
                if (directiveComp.CurrentDirective != null && !directiveComp.IsDirectiveExpired)
                {
                    NavigateCharacter(character, directiveComp);
                    Debug.Log(string.Format("{0} starting directive {1}.", character.name, directiveComp.CurrentDirective.NavNodeName));
                }
                else
                {
                    directiveComp.CurrentDirectiveDuration = DEFAULT_DIRECTIVE_TIMEOUT;
                    Debug.Log(string.Format("{0} waiting for a valid directive.", character.name));
                }
            }
        }
    }

    private void NavigateCharacter(CharacterEntity character, CharacterDirectiveComponent directiveComp)
    {
        NavWorld navWorld = Service.NavWorldManager.CurrentNavWorld;
        string navNetworkName = directiveComp.CurrentDirective.NavNetworkName;
        string navNodeName = directiveComp.CurrentDirective.NavNodeName;

        // Reset any bools that might have been set by node actions.
        character.AnimComponent.ResetToDefaultState();

        NodeNavigationComponent NavComponent = character.NavComponent;
        NavComponent.NavigationQueue = navWorld.Navigate(
            NavComponent.CurrentNavNetwork,
            NavComponent.CurrentNode.name,
            navNetworkName,
            navNodeName);

        NavNetwork targetNetwork =
            Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(navNetworkName);

        NavComponent.FinalDestination = targetNetwork.GetNodeByName(navNodeName);
    }

    private void ChooseNewDirective(CharacterEntity character, CharacterDirectiveComponent directiveComp)
    {
        // Build initial directive list.
        // 1. If any Contexts exist for this character, they are weighed accordingly against
        //    all other top-level directives.
        // 2. If the character is currently engaged in some context, that context's ExitWeight
        //    is used to weigh against other Contexts and top-level directives.
        // 3. If a context is chosen from the top-level selection process, we run a new
        //    selection process on the options contained within that context.
        // 4. Contexts can contain other contexts through the SharedDirectives of
        //    CharacterDirectiveData.
        CharacterDirectiveData directiveData = directiveComp.CurrentDirectiveData;
        List<WeightedDirectiveSelection> weightedDirectives = 
            ResolveNestedDirectives(directiveData, directiveComp);

        float maxRand = GetCurrentWeightRange(weightedDirectives);
        float randomSelection = Random.Range(0f, maxRand);
        WeightedDirectiveSelection selectedDirective = null;
        for (int i = 0, count = weightedDirectives.Count; i < count; ++i)
        {
            if (randomSelection <= weightedDirectives[i].WeightRangeEnd)
            {
                selectedDirective = weightedDirectives[i];
                break;
            }
        }

        if (selectedDirective != null)
        {
            // If a context area was selected, resolve to a single directive.
            CharacterDirective finalDirective = null;
            if (selectedDirective.ContextArea != null)
            {
                float worldTime = Service.TimeOfDay.CurrentDaySeconds;
                List<WeightedDirectiveSelection> currentWeights = new List<WeightedDirectiveSelection>();
                DirectiveContextArea contextArea = selectedDirective.ContextArea;
                directiveComp.CurrentContextArea = contextArea;

                bool directivesInheritBase = contextArea.DirectivesInheritBaseSettings;
                for (int i = 0, count = contextArea.Directives.Count; i < count; ++i)
                {
                    CharacterDirective directive = contextArea.Directives[i];
                    if (directivesInheritBase || 
                        (directive.StartTimeSeconds < worldTime && directive.EndTimeSeconds > worldTime))
                    {
                        if (directivesInheritBase)
                        {
                            directive.NavNetworkName = contextArea.NavNetworkName;
                        }

                        WeightedDirectiveSelection weightedSelection = new WeightedDirectiveSelection();
                        weightedSelection.Directive = directive;
                        float weightRangeStart = GetCurrentWeightRange(currentWeights);
                        weightedSelection.WeightRangeEnd = weightRangeStart + directive.DirectiveWeight;
                        currentWeights.Add(weightedSelection);
                    }
                }

                maxRand = GetCurrentWeightRange(currentWeights);
                randomSelection = Random.Range(0f, maxRand);
                for (int i = 0, count = currentWeights.Count; i < count; ++i)
                {
                    if (randomSelection <= currentWeights[i].WeightRangeEnd)
                    {
                        finalDirective = currentWeights[i].Directive;
                        break;
                    }
                }
            }
            else
            {
                // If no context area was selected, reset current 
                // context area and go directly to the new directive.
                finalDirective = selectedDirective.Directive;
                directiveComp.CurrentContextArea = null;
            }

            if (finalDirective == directiveComp.CurrentDirective)
            {
                Debug.Log("Continuing current directive...");
                return;
            }

            if (finalDirective != null)
            {
                directiveComp.IsDirectiveExpired = false;
                directiveComp.CurrentDirective = finalDirective;
                float duration = Random.Range(finalDirective.MinDuration, finalDirective.MaxDuration);
                directiveComp.CurrentDirectiveDuration = duration;
                Debug.Log("Directive set: " + finalDirective.NavNodeName + " for " + duration + " seconds.");
            }
        }
    }

    private List<WeightedDirectiveSelection> ResolveNestedDirectives(
        CharacterDirectiveData directiveData, 
        CharacterDirectiveComponent directiveComp, 
        List<WeightedDirectiveSelection> currentWeights = null)
    {
        if (currentWeights == null)
        {
            currentWeights = new List<WeightedDirectiveSelection>();
        }

        float worldTime = Service.TimeOfDay.CurrentDaySeconds;

        // Add base directives.
        List<CharacterDirective> baseDirectives = directiveData.BaseDirectives;
        for (int i = 0, count = baseDirectives.Count; i < count; ++i)
        {
            if (baseDirectives[i].StartTimeSeconds <= worldTime && baseDirectives[i].EndTimeSeconds > worldTime)
            {
                WeightedDirectiveSelection selection = new WeightedDirectiveSelection();
                selection.Directive = baseDirectives[i];
                float weightRangeStart = GetCurrentWeightRange(currentWeights);
                selection.WeightRangeEnd = weightRangeStart + baseDirectives[i].DirectiveWeight;
                currentWeights.Add(selection);
            }
        }

        List<DirectiveContextArea> contextAreas = directiveData.DirectiveContextAreas;
        for (int i = 0, count = contextAreas.Count; i < count; ++i)
        {
            if (contextAreas[i].StartTimeSeconds <= worldTime && contextAreas[i].EndTimeSeconds > worldTime)
            {
                WeightedDirectiveSelection selection = new WeightedDirectiveSelection();
                selection.ContextArea = contextAreas[i];
                float weightRangeStart = GetCurrentWeightRange(currentWeights);
                float weight = contextAreas[i] == directiveComp.CurrentContextArea ?
                    contextAreas[i].ExitWeight : contextAreas[i].EntryWeight;

                selection.WeightRangeEnd = weightRangeStart + weight;
                currentWeights.Add(selection);
            }
        }

        if (directiveData.SharedDirectives != null && directiveData.SharedDirectives.Count > 0)
        {
            List<CharacterDirectiveData> sharedDirectives = directiveData.SharedDirectives;
            for (int i = 0, count = sharedDirectives.Count; i < count; ++i)
            {
                ResolveNestedDirectives(sharedDirectives[i], directiveComp, currentWeights);
            }
        }

        return currentWeights;
    }

    private float GetCurrentWeightRange(List<WeightedDirectiveSelection> currentWeights)
    {
        return currentWeights.Count > 0f ?
                currentWeights[currentWeights.Count - 1].WeightRangeEnd : 0f;
    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
