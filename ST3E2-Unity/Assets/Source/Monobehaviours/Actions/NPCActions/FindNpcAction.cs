using UnityEngine;

public class FindNpcAction : CustomAction
{
    public string NpcName;
    public CustomAction OnNpcFound;
    public CustomAction OnNpcNotFound;
    private CharacterEntity characterEntity;

    public override void Initiate()
    {
        GameObject npcObj = GameObject.Find(NpcName);
        if (npcObj != null)
        {
            characterEntity = npcObj.GetComponent<CharacterEntity>();
        }

        if (npcObj == null || characterEntity == null)
        {
            if (OnNpcNotFound != null)
            {
                OnNpcNotFound.Initiate();
            }
            else
            {
                Debug.LogWarning("No NPC found named " + NpcName);
            }
        }

        if (OnNpcFound != null && OnNpcFound != null)
        {
            OnNpcFound.Initiate();
        }
    }

    public CharacterEntity GetTargetEntity()
    {
        return characterEntity;
    }
}
