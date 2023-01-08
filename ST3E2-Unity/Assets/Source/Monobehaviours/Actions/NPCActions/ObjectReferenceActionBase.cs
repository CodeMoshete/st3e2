using UnityEngine;

public class ObjectReferenceActionBase : CustomAction
{
    protected InteractiveLevelContent levelContent;

    private void Start()
    {
        levelContent = GameObject.FindObjectOfType<InteractiveLevelContent>();
        Service.EventManager.AddListener(EventId.CurrentNavNetworkChanged, OnNavNetworkChanged);
    }

    private bool OnNavNetworkChanged(object cookie)
    {
        levelContent = GameObject.FindObjectOfType<InteractiveLevelContent>();
        return false;
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.CurrentNavNetworkChanged, OnNavNetworkChanged);
    }
}
