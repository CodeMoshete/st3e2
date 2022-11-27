using System.Collections.Generic;

public class NodeNavigationComponent
{
    public Queue<NavNode> NavigationQueue;
    public NavNode FinalDestination;
    public float WalkRate;
    public float TurnRate;
}
