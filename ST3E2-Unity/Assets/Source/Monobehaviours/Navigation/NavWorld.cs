using System.Collections.Generic;
using UnityEngine;

public class NavWorld : MonoBehaviour
{
    private List<NavNetwork> networks;
    public List<NavNetwork> Networks
    {
        get
        {
            if (networks == null)
            {
                networks = new List<NavNetwork>(gameObject.GetComponentsInChildren<NavNetwork>());
            }
            return networks;
        }
    }

    public NavNetwork GetNetworkByName(string networkName)
    {
        for (int i = 0, count = networks.Count; i < count; ++i)
        {
            if (networks[i].NetworkName == networkName)
            {
                return networks[i];
            }
        }
        return null;
    }

    public Queue<NavNode> Navigate(string networkName, string sourceName, string destName)
    {
        NavNetwork network = GetNetworkByName(networkName);
        if (network != null)
        {
            return network.Navigate(sourceName, destName);
        }
        return null;
    }

    public Queue<NavNode> Navigate(string sourceNetworkName, string sourceName, 
        string destNetworkName, string destName)
    {
        NavNetwork sourceNetwork = GetNetworkByName(sourceNetworkName);
        NavNetwork destNetwork = GetNetworkByName(destNetworkName);
        if (sourceNetwork != null && destNetwork != null)
        {
            Queue<NavNode> frontPortion = sourceNetwork.NavigateToExitNode(sourceName);
            Queue<NavNode> fullQueue = new Queue<NavNode>();
            string exitTag = null;
            while (frontPortion.Count > 0)
            {
                if (frontPortion.Count == 1)
                {
                    exitTag = frontPortion.Peek().ExitNodeTag;
                }
                fullQueue.Enqueue(frontPortion.Dequeue());
            }

            Queue<NavNode> backPortion = destNetwork.NavigateFromExitNode(destName, exitTag);
            while (backPortion.Count > 0)
            {
                fullQueue.Enqueue(backPortion.Dequeue());
            }
            return fullQueue;

        }
        return null;
    }

    public Queue<NavNode> Navigate(string networkName, NavNode source, NavNode destination)
    {
        NavNetwork network = GetNetworkByName(networkName);
        if (network != null)
        {
            return network.Navigate(source, destination);
        }
        return null;
    }
}
