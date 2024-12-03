using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConversationData", menuName = "ScriptableObjects/CharacterConversationData")]
public class CharacterConversationData : ScriptableObject
{
    public List<ConversationCondition> Conditions;
    public string ConversationResourceName;
}
