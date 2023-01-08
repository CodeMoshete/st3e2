using System.Collections.Generic;
using UnityEngine;

public class InteractiveLevelContent : MonoBehaviour
{
    public List<GameObject> InteractiveObjects;
    private Dictionary<string, GameObject> interactiveObjectsByName;
    private Dictionary<string, GameObject> InteractiveObjectsByName
    {
        get
        {
            if (interactiveObjectsByName == null)
            {
                interactiveObjectsByName = new Dictionary<string, GameObject>();

                for (int i = 0, count = InteractiveObjects.Count; i < count; ++i)
                {
                    interactiveObjectsByName.Add(InteractiveObjects[i].name, InteractiveObjects[i]);
                }
            }

            return interactiveObjectsByName;
        }
    }

    public GameObject GetInteractiveObject(string name)
    {
        if (InteractiveObjectsByName.ContainsKey(name))
        {
            return InteractiveObjectsByName[name];
        }
        return null;
    }
}
