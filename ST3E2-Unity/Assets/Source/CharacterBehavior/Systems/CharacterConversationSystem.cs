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
            EvaluateAndPopulateConversations(character);
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
            // Debug.Log("Checking " + character.name);
            if (character.IsViewVisible && character.ConversationComponent.IsConversationsActive)
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

    private void EvaluateAndPopulateConversations(CharacterEntity character)
    {
        CharacterConversationComponent convoComp = character.ConversationComponent;

        convoComp.PlayerInitiatedConversations = new List<CharacterConversationData>();
        convoComp.NpcInitiatedConversations = new List<CharacterConversationData>();

        if (convoComp.AllConversations == null)
        {
            return;
        }

        for (int i = 0, count = convoComp.AllConversations.Count; i < count; ++i)
        {
            CharacterConversationData convoData = convoComp.AllConversations[i];
            bool meetsConditions = true;
            for (int j = 0, conditionCt = convoData.Conditions.Count; j < conditionCt; ++j)
            {
                ConversationCondition condition = convoData.Conditions[j];

                if (j > 0 && condition.OrWithPrevious && meetsConditions)
                {
                    continue;
                }

                bool conditionMet = false;
                switch (condition.Category)
                {
                    case ConditionCategory.PlayerStat:
                        conditionMet = EvaluatePlayerStat(condition);
                        break;
                    case ConditionCategory.CurrentArea:
                        conditionMet = EvaluateCurrentArea(condition);
                        break;
                    case ConditionCategory.TimeOfDay:
                        conditionMet = EvaluateTimeOfDay(condition);
                        break;
                }

                if (j > 0 && condition.OrWithPrevious && !meetsConditions)
                {
                    meetsConditions = conditionMet;
                }

                if (!condition.OrWithPrevious && !conditionMet)
                {
                    meetsConditions = false;
                    break;
                }
            }

            if (meetsConditions)
            {
                switch(convoData.InitiationType)
                {
                    case ConversationInitiationType.Player:
                        convoComp.PlayerInitiatedConversations.Add(convoData);
                        break;
                    case ConversationInitiationType.Npc:
                        convoComp.NpcInitiatedConversations.Add(convoData);
                        break;
                }
            }
        }
    }

    private bool EvaluatePlayerStat(ConversationCondition condition)
    {
        bool conditionMet = false;
        int statValue = Service.PlayerData.GetStat(condition.Key);
        int testValue;
        int.TryParse(condition.Value, out testValue);
        switch(condition.Comparison)
        {
            case ComparisonType.Equals:
                conditionMet = statValue == testValue;
                break;
            case ComparisonType.GreaterThan:
                conditionMet = statValue > testValue;
                break;
            case ComparisonType.GreaterThanEqualTo:
                conditionMet = statValue >= testValue;
                break;
            case ComparisonType.LessThan:
                conditionMet = statValue < testValue;
                break;
            case ComparisonType.LessThanEqualTo:
                conditionMet = statValue <= testValue;
                break;
        }
        return conditionMet;
    }

    private bool EvaluateCurrentArea(ConversationCondition condition)
    {
        return Service.NavWorldManager.CurrentNavWorld.WorldID.ToString() == condition.Value;
    }

    private bool EvaluateTimeOfDay(ConversationCondition condition)
    {
        bool conditionMet = false;
        float currentTime = Service.TimeOfDay.CurrentDaySeconds;
        float testValue = Service.TimeOfDay.FormattedTimeToSeconds(condition.Value);
        switch(condition.Comparison)
        {
            case ComparisonType.Equals:
                conditionMet = currentTime == testValue;
                break;
            case ComparisonType.GreaterThan:
                conditionMet = currentTime > testValue;
                break;
            case ComparisonType.GreaterThanEqualTo:
                conditionMet = currentTime >= testValue;
                break;
            case ComparisonType.LessThan:
                conditionMet = currentTime < testValue;
                break;
            case ComparisonType.LessThanEqualTo:
                conditionMet = currentTime <= testValue;
                break;
        }
        return conditionMet;
    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
