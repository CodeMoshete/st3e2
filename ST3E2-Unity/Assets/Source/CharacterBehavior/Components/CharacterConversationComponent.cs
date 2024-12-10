using System.Collections.Generic;

public class CharacterConversationComponent
{
    public bool IsConversationsActive;
    public bool IsInRange;
    public List<CharacterConversationData> AllConversations;
    public List<CharacterConversationData> NpcInitiatedConversations;
    public List<CharacterConversationData> PlayerInitiatedConversations;
}
