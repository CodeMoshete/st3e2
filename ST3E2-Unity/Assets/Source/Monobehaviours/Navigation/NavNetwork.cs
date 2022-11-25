using System.Collections.Generic;
using UnityEngine;

public class NavNetwork : MonoBehaviour
{
    public NavNode StartNode;
    public NavNode DestNode;

    private List<NavNode> nodes = new List<NavNode>();

    // Collections used for navigational purposes.
    private HashSet<NavNode> seenNodes = new HashSet<NavNode>();
    private List<NavPath> possiblePaths = new List<NavPath>();

    private void Start()
    {
        nodes = new List<NavNode>(gameObject.GetComponentsInChildren<NavNode>());
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (nodes.Count == 0)
        {
            Debug.Log("Test");
            nodes = new List<NavNode>(gameObject.GetComponentsInChildren<NavNode>());
        }

        Gizmos.color = Color.red;
        for (int i = 0, count = nodes.Count; i < count; ++i)
        {
            for (int j = 0, count2 = nodes[i].Links.Count; j < count2; ++j)
            {
                NavNodeLink link = nodes[i].Links[j];
                //Draw the suspension
                Gizmos.DrawLine(nodes[i].transform.position, link.DestinationNode.transform.position);
            }
        }
    }
#endif

    public List<NavNodeLink> Navigate(string sourceName, string destName)
    {
        return Navigate(GetNodeByName(sourceName), GetNodeByName(destName));
    }

    public List<NavNodeLink> Navigate(NavNode source, NavNode destination)
    {
        for (int i = 0, count = source.Links.Count; i < count; ++i)
        {
            NavPath basePathVariant = new NavPath();

            if (seenNodes.Contains(source.Links[i].DestinationNode))
            {
                Debug.LogError("Duplicate node link: " + source.name + " - " + 
                    source.Links[i].DestinationNode.name);
                continue;
            }

            basePathVariant.AddLink(source.Links[i]);
            seenNodes.Add(source.Links[i].DestinationNode);
            possiblePaths.Add(basePathVariant);
            NavNodeLink nextLink = source.Links[i];

            if (nextLink.DestinationNode == destination)
            {
                basePathVariant.isAtDestination = true;
            }
            else if (basePathVariant.isViable)
            {
                SearchNodeNetwork(basePathVariant, destination);
            }
        }

        Debug.Log("Number of possible paths: " + possiblePaths.Count);
        NavPath optimalPath = null;
        for (int i = 0, count = possiblePaths.Count; i < count; ++i)
        {
            if (possiblePaths[i].isAtDestination)
            {
                if (optimalPath == null || possiblePaths[i].TotalLength < optimalPath.TotalLength)
                {
                    optimalPath = possiblePaths[i];
                }
            }
        }
        possiblePaths.Clear();

        if (optimalPath != null)
        {
            Debug.Log(optimalPath.ToString());
        }

        return optimalPath.NodeLinks;
    }

    private void SearchNodeNetwork(NavPath currentPath, NavNode destination)
    {
        NavNode headNode = currentPath.NodeLinks[currentPath.NodeLinks.Count - 1].DestinationNode;
        for(int i = 0, count = headNode.Links.Count; i < count; ++i)
        {
            NavPath path = currentPath;
            if (i < count - 1)
            {
                path = currentPath.CreateDuplicate();
                possiblePaths.Add(path);
            }

            NavNodeLink nextLink = headNode.Links[i];
            path.AddLink(nextLink);
            
            if (nextLink.DestinationNode == destination)
            {
                path.isAtDestination = true;
            }
            else if(path.isViable)
            {
                if (seenNodes.Contains(nextLink.DestinationNode))
                {
                    ResolveDuplicatePaths(path);
                    continue;
                }

                if (path.isViable)
                {
                    SearchNodeNetwork(path, destination);
                }
            }
        }
    }

    private void ResolveDuplicatePaths(NavPath testPath)
    {
        NavNode testNode = testPath.NodeLinks[testPath.NodeLinks.Count - 1].DestinationNode;
        for (int i = 0, count = possiblePaths.Count; i < count; ++i)
        {
            Dictionary<NavNode, float> nodeDistances = possiblePaths[i].NodeDistances;
            if (nodeDistances.ContainsKey(testNode))
            {
                if (nodeDistances[testNode] > testPath.TotalLength)
                {
                    possiblePaths[i].isViable = false;
                }
                else
                {
                    testPath.isViable = false;
                }
                // There should only ever be one optimal path to a given node at a time this way.
                return;
            }
        }
    }

    public NavNode GetNodeByName(string name)
    {
        for (int i = 0, count = nodes.Count; i < count; ++i)
        {
            if (nodes[i].name == name)
            {
                return nodes[i];
            }
        }
        return null;
    }
}
