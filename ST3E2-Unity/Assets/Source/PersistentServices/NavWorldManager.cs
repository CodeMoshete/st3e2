using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavWorldManager : MonoBehaviour
{
    private static NavWorldManager instance;
    public static NavWorldManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject newInstance = GameObject.Instantiate(Resources.Load<GameObject>("NavWorldManager"));
                instance = newInstance.GetComponent<NavWorldManager>();
            }

            return instance;
        }
    }

    public NavWorld CurrentNavWorld;

    private Dictionary<NavWorldID, string> NavIdPrefabMap = new Dictionary<NavWorldID, string>();

    public NavWorldManager()
    {
        NavIdPrefabMap.Add(NavWorldID.Monitor, "NavWorlds/MonitorNavigation");
    }

    public void SetNavWorldAndNetwork(NavWorldID worldId, string networkName)
    {
        LoadNavWorld(worldId);
        CurrentNavWorld.SetActiveNetworkByName(networkName);
    }

    public void LoadNavWorld(NavWorldID worldId)
    {
        if (CurrentNavWorld != null && CurrentNavWorld.WorldID == worldId)
        {
            return;
        }

        if (NavIdPrefabMap.ContainsKey(worldId))
        {
            if (CurrentNavWorld != null && CurrentNavWorld.WorldID != worldId)
            {
                GameObject.Destroy(CurrentNavWorld.gameObject);
            }

            GameObject newNavWorld = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(NavIdPrefabMap[worldId]));
            CurrentNavWorld = newNavWorld.GetComponent<NavWorld>();

            Service.EventManager.SendEvent(EventId.NavWorldChanged, CurrentNavWorld.WorldID);
        }
    }
}
