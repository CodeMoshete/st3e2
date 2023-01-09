using System.Collections.Generic;
using UnityEngine;

public class NavNetwork : MonoBehaviour
{
    public string NetworkName;

    [HideInInspector]
    private List<NavNode> cachedExitNodes;
    public List<NavNode> ExitNodes
    {
        get
        {
            if (cachedExitNodes == null)
            {
                cachedExitNodes = new List<NavNode>();
                for (int i = 0, count = nodes.Count; i < count; ++i)
                {
                    if (!string.IsNullOrEmpty(nodes[i].ExitNodeTag))
                    {
                        cachedExitNodes.Add(nodes[i]);
                    }
                }
            }
            return cachedExitNodes;
        }
    }

    private List<NavNode> cachedNodes;
    private List<NavNode> nodes
    {
        get
        {
            if (cachedNodes == null)
            {
                cachedNodes = new List<NavNode>(gameObject.GetComponentsInChildren<NavNode>());

                for (int i = 0, count = cachedNodes.Count; i < count; ++i)
                {
                    cachedNodes[i].Initialize(NetworkName);
                }
            }
            return cachedNodes;
        }
    }

    // Collections used for navigational purposes.
    private HashSet<NavNode> seenNodes = new HashSet<NavNode>();
    private List<NavPath> possiblePaths = new List<NavPath>();

    private NavPath currentOptimalPath;

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (cachedNodes == null)
        {
            cachedNodes = new List<NavNode>(gameObject.GetComponentsInChildren<NavNode>());
        }

        for (int i = 0, count = nodes.Count; i < count; ++i)
        {
            Gizmos.color = Color.red;
            for (int j = 0, count2 = nodes[i].Links.Count; j < count2; ++j)
            {
                NavNodeLink link = nodes[i].Links[j];
                if (link != null && link.DestinationNode != null)
                {
                    Gizmos.DrawLine(nodes[i].transform.position, link.DestinationNode.transform.position);
                }
            }

            Gizmos.color = Color.green;
            Vector3 nodePos = nodes[i].transform.position;
            float triggerRadius = nodes[i].TriggerRadius;
            int steps = 12;
            for (int j = 0; j < steps; ++j)
            {
                float pct = ((float)j / (float)steps);
                Vector3 startPoint = new Vector3(
                    nodePos.x + Mathf.Sin(pct * 2 * Mathf.PI) * triggerRadius,
                    nodePos.y,
                    nodePos.z + Mathf.Cos(pct * 2 * Mathf.PI) * triggerRadius);

                float nextStep = j < steps - 1 ? j + 1 : 0;
                float nextPct = ((float)nextStep / (float)steps);
                Vector3 nextPoint = new Vector3(
                    nodePos.x + Mathf.Sin(nextPct * 2 * Mathf.PI) * triggerRadius,
                    nodePos.y,
                    nodePos.z + Mathf.Cos(nextPct * 2 * Mathf.PI) * triggerRadius);

                Gizmos.DrawLine(startPoint, nextPoint);
            }
        }
    }
#endif

    public Queue<NavNode> Navigate(string sourceName, string destName)
    {
        return Navigate(GetNodeByName(sourceName), GetNodeByName(destName));
    }

    public Queue<NavNode> Navigate(NavNode source, NavNode destination)
    {
        Debug.Log("[NavNetwork] Calculate navigation from " + source.name + " to " + destination.name);
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

        NavPath optimalPath = currentOptimalPath;
        possiblePaths.Clear();
        seenNodes.Clear();
        currentOptimalPath = null;

        if (optimalPath != null)
        {
            Debug.Log(optimalPath.ToString());
        }

        return optimalPath.GetNodes();
    }

    public Queue<NavNode> NavigateToExitNode(string sourceName)
    {
        return NavigateToExitNode(GetNodeByName(sourceName));
    }

    public Queue<NavNode> NavigateToExitNode(NavNode source)
    {
        NavNode exitNode = GetClosestExitNode(source);
        if (exitNode != null)
        {
            return Navigate(source, exitNode);
        }
        return null;
    }

    public Queue<NavNode> NavigateFromExitNode(string destName, string exitNodeTag = null)
    {
        return NavigateFromExitNode(GetNodeByName(destName), exitNodeTag);
    }

    public Queue<NavNode> NavigateFromExitNode(NavNode dest, string exitNodeTag = null)
    {
        NavNode exitNode = null;
        if (!string.IsNullOrEmpty(exitNodeTag))
        {
            exitNode = GetExitNodeByTag(exitNodeTag);
        }
        else
        {
            exitNode = ExitNodes.Count > 0 ? ExitNodes[Random.Range(0, ExitNodes.Count)] : null;
        }

        if (exitNode != null)
        {
            return Navigate(exitNode, dest);
        }
        return null;
    }

    private NavNode GetClosestExitNode(NavNode source)
    {
        NavNode closestExitNode = null;
        float closestDistance = float.MaxValue;
        for (int i = 0, count = ExitNodes.Count; i < count; ++i)
        {
            Vector3 sourcePos = source.transform.position;
            float thisDist = Vector3.SqrMagnitude(ExitNodes[i].transform.position - sourcePos);
            if (thisDist < closestDistance)
            {
                closestDistance = thisDist;
                closestExitNode = ExitNodes[i];
            }
        }
        return closestExitNode;
    }

    private NavNode GetExitNodeByTag(string exitNodeTag)
    {
        for (int i = 0, count = ExitNodes.Count; i < count; ++i)
        {
            if (ExitNodes[i].ExitNodeTag == exitNodeTag)
            {
                return ExitNodes[i];
            }
        }
        return null;
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

            if (currentOptimalPath != null && path.TotalLength > currentOptimalPath.TotalLength)
            {
                path.isViable = false;
                continue;
            }
            
            if (nextLink.DestinationNode == destination)
            {
                path.isAtDestination = true;
                if (currentOptimalPath == null || path.TotalLength < currentOptimalPath.TotalLength)
                {
                    currentOptimalPath = path;
                }
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
