using System.Collections.Generic;

public class NavPath
{
    public List<NavNodeLink> NodeLinks = new List<NavNodeLink>();
    public Dictionary<NavNode, float> NodeDistances = new Dictionary<NavNode, float>();
    public float TotalLength = 0f;
    public bool isViable = true;
    public bool isAtDestination = false;

    public void AddLink(NavNodeLink link)
    {
        if (!NodeDistances.ContainsKey(link.DestinationNode))
        {
            NodeLinks.Add(link);
            TotalLength += link.Distance;
            NodeDistances[link.DestinationNode] = TotalLength;
        }
        else
        {
            // Path loops back around on itself.
            isViable = false;
        }
    }

    public NavPath CreateDuplicate()
    {
        NavPath duplicate = new NavPath();

        for (int i = 0, count = NodeLinks.Count; i < count; ++i)
        {
            duplicate.AddLink(NodeLinks[i]);
        }

        return duplicate;
    }

    public override string ToString()
    {
        string output = string.Empty;
        if (NodeLinks.Count > 0)
        {
            output += "[ " + NodeLinks[0].SourceNode.name;
            for (int i = 0, count = NodeLinks.Count; i < count; ++i)
            {
                output += NodeLinks[i].DestinationNode.name;
            }
            output += " ]\nTotal Length: " + TotalLength;
        }
        return output;
    }

    public Queue<NavNode> GetNodes()
    {
        Queue<NavNode> nodes = new Queue<NavNode>();
        if (NodeLinks.Count > 0)
        {
            nodes.Enqueue(NodeLinks[0].SourceNode);
            for (int i = 0, count = NodeLinks.Count; i < count; ++i)
            {
                nodes.Enqueue(NodeLinks[i].DestinationNode);
            }
        }
        return nodes;
    }
}
