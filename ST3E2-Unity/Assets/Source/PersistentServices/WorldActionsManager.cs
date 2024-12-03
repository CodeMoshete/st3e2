using UnityEngine;

public class WorldActionsManager
{
    private static WorldActionsManager managerInstance;
    public static WorldActionsManager Instance
    {
        get
        {
            if (managerInstance == null)
            {
                managerInstance = new WorldActionsManager();
            }
            return managerInstance;
        }
    }

    private GameObject currentWorldActions;

    public void LoadAndSetWorldActions(string sceneActionsResourceName)
    {
        ClearWorldActions();

        GameObject prototype = Resources.Load<GameObject>(sceneActionsResourceName);
        currentWorldActions = GameObject.Instantiate<GameObject>(prototype);
        Object.DontDestroyOnLoad(currentWorldActions);
    }

    public void ClearWorldActions()
    {
        if (currentWorldActions != null)
        {
            Object.Destroy(currentWorldActions);
            currentWorldActions = null;
        }
    }
}
