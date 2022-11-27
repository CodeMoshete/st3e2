using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public bool CalculateNewPath = false;
    public string SourceNodeName;
    public string DestNodeName;

    // Components are added and removed by systems.
    [HideInInspector]
    public NodeNavigationComponent NavComponent;
    [HideInInspector]
    public Animator AnimComponent;

    private void Start()
    {
        Service.CharacterSystems.Navigation.AddCharacter(this);
        NavComponent.TurnRate = 45f;
        NavComponent.WalkRate = 1f;

        AnimComponent = GetComponent<Animator>();

        NavNetwork navNetwork = GameObject.Find("Navigation").GetComponent<NavNetwork>();
        NavComponent.NavigationQueue = navNetwork.Navigate("Node", "Node (5)");
        NavComponent.FinalDestination = navNetwork.GetNodeByName("Node (5)");
    }

    private void Update()
    {
        if (CalculateNewPath)
        {
            NavNetwork navNetwork = GameObject.Find("Navigation").GetComponent<NavNetwork>();
            CalculateNewPath = false;
            NavComponent.NavigationQueue = navNetwork.Navigate(SourceNodeName, DestNodeName);
            NavComponent.FinalDestination = navNetwork.GetNodeByName(DestNodeName);
        }
    }
}
