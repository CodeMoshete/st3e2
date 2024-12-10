using System;
using System.Collections.Generic;

public class NodeNavigationComponent
{
    public Queue<NavNode> NavigationQueue;
    public NavNode FinalDestination;
    public NavNode CurrentNode;
    public float WalkRate;
    public float TurnRate;
    public string CurrentNavNetwork;
    public Action OnFinalDestinationReached;
    public bool IsNavigationEnabled = true;

    public bool IsInTransit
    {
        get
        {
            return FinalDestination != null;
        }
    }
}
