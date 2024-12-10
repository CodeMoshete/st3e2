using System.Collections.Generic;
using UnityEngine;

public class NavigateToNodeAction : NpcBaseAction
{
    public string NavNetworkName;
    public List<string> NavNodeNames;
    public bool CancelIfOccupied;
    public CustomAction NextAction;
    public CustomAction OnArrivedAtDestination;

    public override void Initiate()
    {
        base.Initiate();

        List<NavNode> validNodes = new List<NavNode>();
        for (int i = 0, count = NavNodeNames.Count; i < count; ++i)
        {
            NavNetwork targetNetwork =
            Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(NavNetworkName);

            NavNode node = targetNetwork.GetNodeByName(NavNodeNames[i]);

            NavNodeAttribute occupied = node.GetAttribute("occupied");
            if (!CancelIfOccupied || occupied == null || !occupied.Value)
            {
                validNodes.Add(node);
            }
        }

        int numValidNodes = validNodes.Count;
        if (numValidNodes == 0)
        {
            PerformNextAction();
            return;
        }

        NavNode targetNode = validNodes[Random.Range(0, numValidNodes)];

        NavWorld navWorld = Service.NavWorldManager.CurrentNavWorld;
        NodeNavigationComponent NavComponent = TargetEntity.NavComponent;
        NavComponent.NavigationQueue = navWorld.Navigate(
            NavComponent.CurrentNavNetwork,
            NavComponent.CurrentNode.name,
            NavNetworkName,
            targetNode.name);

        if (OnArrivedAtDestination != null)
        {
            NavComponent.OnFinalDestinationReached = OnFinalDestinationReached;
        }
        NavComponent.FinalDestination = targetNode;

        PerformNextAction();
    }

    private void OnFinalDestinationReached()
    {
        OnArrivedAtDestination.Initiate();
    }

    private void PerformNextAction()
    {
        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
