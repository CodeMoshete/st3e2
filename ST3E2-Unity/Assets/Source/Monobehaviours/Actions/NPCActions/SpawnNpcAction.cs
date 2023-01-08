using UnityEngine;

public class SpawnNpcAction : CustomAction
{
    public string EntityResourcePath;
    public string NavNetworkName;
    public string StartNavNodeName;
    public CustomAction NextAction;

    public override void Initiate()
    {
        GameObject npc = GameObject.Instantiate(Resources.Load<GameObject>(EntityResourcePath));
        NavNetwork navNetwork = Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(NavNetworkName);
        NavNode startNode = navNetwork.GetNodeByName(StartNavNodeName);

        npc.transform.position = startNode.transform.position;
        CharacterEntity character = npc.GetComponent<CharacterEntity>();
        character.Initialize();
        character.NavComponent.CurrentNavNetwork = NavNetworkName;
        character.NavComponent.CurrentNode = startNode;

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
