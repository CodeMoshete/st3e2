using System.Collections.Generic;

public class CharacterConversationComponent
{
    public bool IsInRange;
    public List<CharacterConversationData> AllConversations;
    public CharacterConversationData NpcInitiatedConversation;
    public List<CharacterConversationData> PlayerInitiatedConversations;
}
