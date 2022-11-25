using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    public List<NavNodeLink> Links;

    private void Start()
    {
        for (int i = 0, count = Links.Count; i < count; ++i)
        {
            Links[i].SourceNode = this;
        }
    }
}
