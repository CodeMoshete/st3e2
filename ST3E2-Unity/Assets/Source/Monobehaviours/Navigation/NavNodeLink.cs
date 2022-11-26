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
                    Debug.LogError("Null node");
                }

                Vector3 directionVector = DestinationNode.transform.position - SourceNode.transform.position;
                cachedDistance = Vector3.SqrMagnitude(directionVector);
            }
            return cachedDistance;
        }
    }

    public NavNode DestinationNode;
}
