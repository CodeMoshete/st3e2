using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    public List<NavNodeLink> Links;
    public float TriggerRadius = 1f;

    public void Initialize()
    {
        for (int i = 0, count = Links.Count; i < count; ++i)
        {
            Links[i].SourceNode = this;
        }
    }
}
