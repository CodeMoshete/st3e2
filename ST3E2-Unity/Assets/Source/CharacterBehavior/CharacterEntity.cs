using System.Collections.Generic;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public bool CalculateNewPath = false;
    public string DestNodeName;

    // Components are added and removed by systems.
    [HideInInspector]
    public NodeNavigationComponent NavComponent;
    [HideInInspector]
    public Animator AnimComponent;
    [HideInInspector]
    public CharacterDirectiveComponent DirectiveComponent;
    public List<CharacterDirective> DefaultDirectives;


    private void Start()
    {
        Service.CharacterSystems.Navigation.AddCharacter(this);
        NavComponent.TurnRate = 70f;
        NavComponent.WalkRate = 1f;

        AnimComponent = GetComponent<Animator>();

        NavNetwork navNetwork = GameObject.Find("Deck3").GetComponent<NavNetwork>();
        NavComponent.CurrentNode = navNetwork.GetNodeByName("Node (11)"); // Set start node.
        //NavComponent.NavigationQueue = navNetwork.Navigate("Node", "Node (5)");
        //NavComponent.FinalDestination = navNetwork.GetNodeByName("Node (5)");
    }

    private void Update()
    {
        if (CalculateNewPath)
        {
            NavNetwork navNetwork = GameObject.Find("Deck3").GetComponent<NavNetwork>();
            CalculateNewPath = false;
            NavComponent.NavigationQueue = navNetwork.Navigate(NavComponent.CurrentNode.name, DestNodeName);
            NavComponent.FinalDestination = navNetwork.GetNodeByName(DestNodeName);
        }
    }
}
