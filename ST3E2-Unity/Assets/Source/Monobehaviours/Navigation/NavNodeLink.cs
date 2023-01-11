using UnityEngine;

[System.Serializable]
public class NavNodeLink
{
    [HideInInspector]
    public NavNode SourceNode;

    private float cachedDistance;
    [HideInInspector]
    public float Distance
    {
        get
        {
            if (cachedDistance == 0f)
            {
                if (SourceNode == null || DestinationNode == null)
                {
                    NavNode activeNode = SourceNode != null ? SourceNode : DestinationNode;
                    Debug.LogError("Null node from source node " + activeNode?.name + " in network " + activeNode?.ParentNetworkName);
                }

                Vector3 directionVector = DestinationNode.transform.position - SourceNode.transform.position;
                cachedDistance = Vector3.SqrMagnitude(directionVector);
            }
            return cachedDistance;
        }
    }

    public NavNode DestinationNode;
}
