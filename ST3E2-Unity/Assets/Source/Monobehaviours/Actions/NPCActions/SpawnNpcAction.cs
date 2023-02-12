using UnityEngine;

public class SpawnNpcAction : CustomAction
{
    public string EntityResourcePath;
    public bool SpawnOnlyIfNotExists;
    public string NavNetworkName;
    public string StartNavNodeName;
    public CustomAction NextAction;

    public override void Initiate()
    {
        if (SpawnOnlyIfNotExists && 
            Service.NavWorldManager.CurrentNavWorld.GetCharacterIsRegistered(EntityResourcePath))
        {
            return;
        }

        GameObject npc = GameObject.Instantiate(Resources.Load<GameObject>(EntityResourcePath));
        NavNetwork navNetwork = Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(NavNetworkName);
        NavNode startNode = navNetwork.GetNodeByName(StartNavNodeName);

        npc.transform.position = startNode.transform.position;
        CharacterEntity character = npc.GetComponent<CharacterEntity>();
        character.Initialize();
        character.NavComponent.CurrentNavNetwork = NavNetworkName;
        character.NavComponent.CurrentNode = startNode;

        Service.CharacterSystems.Navigation.AddCharacter(character);
        Service.CharacterSystems.Directives.AddCharacter(character);

        Service.NavWorldManager.CurrentNavWorld.RegisterCharacter(character);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
