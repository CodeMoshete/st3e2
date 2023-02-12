using System.Collections.Generic;
using UnityEngine;
using Utils;

public class NavWorld : MonoBehaviour
{
    public NavWorldID WorldID;
    public NavNetwork ActiveNetwork { get; private set; }

    private List<CharacterEntity> activeCharacters = new List<CharacterEntity>();
    public List<CharacterEntity> ActiveCharacters
    {
        get
        {
            return activeCharacters;
        }
    }

    private List<NavNetwork> networks;
    public List<NavNetwork> Networks
    {
        get
        {
            if (networks == null)
            {
                networks = UnityUtils.FindAllComponentsInChildren<NavNetwork>(gameObject);
            }
            return networks;
        }
    }

    public void SetActiveNetworkByName(string networkName)
    {
        NavNetwork nextNetwork = GetNetworkByName(networkName);
        if (nextNetwork == ActiveNetwork)
        {
            return;
        }

        if (ActiveNetwork != null)
        {
            ActiveNetwork.gameObject.SetActive(false);
        }

        ActiveNetwork = nextNetwork;
        ActiveNetwork.gameObject.SetActive(true);

        Service.EventManager.SendEvent(EventId.CurrentNavNetworkChanged, ActiveNetwork.name);
    }

    public NavNetwork GetNetworkByName(string networkName)
    {
        for (int i = 0, count = Networks.Count; i < count; ++i)
        {
            if (Networks[i].NetworkName == networkName)
            {
                return Networks[i];
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
        if (sourceNetwork != null && destNetwork != null && sourceNetwork != destNetwork)
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
        else if (sourceNetwork != null && sourceNetwork == destNetwork)
        {
            return sourceNetwork.Navigate(sourceName, destName);
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

    public void RegisterCharacter(CharacterEntity character)
    {
        ActiveCharacters.Add(character);
    }

    public void UnregisterCharacter(CharacterEntity character)
    {
        ActiveCharacters.Remove(character);
    }

    public bool GetCharacterIsRegistered(string characterResourcePath)
    {
        string[] baseNameParts = characterResourcePath.Split('/');
        string baseName = string.Format("{0}(Clone)", baseNameParts[baseNameParts.Length - 1]);
        for (int i = 0, count = ActiveCharacters.Count; i < count; ++i)
        {
            if (ActiveCharacters[i].name == baseName)
            {
                return true;
            }
        }
        return false;
    }

    public void OnDestroy()
    {
        for (int i = 0, count = ActiveCharacters.Count; i < count; ++i)
        {
            ActiveCharacters[i].Dispose();
        }

        activeCharacters = null;
    }
}
