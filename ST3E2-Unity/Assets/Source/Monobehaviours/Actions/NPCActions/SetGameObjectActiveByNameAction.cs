using System.Collections.Generic;
using UnityEngine;

public class SetGameObjectActiveByNameAction : ObjectReferenceActionBase
{
    public List<string> TargetNames;
    public CustomAction OnDone;
    public bool SetActive;

    public override void Initiate()
    {
        for (int i = 0, count = TargetNames.Count; i < count; ++i)
        {
            GameObject targetObject = levelContent.GetInteractiveObject(TargetNames[i]);
            if (targetObject != null)
            {
                targetObject.SetActive(SetActive);
            }
        }

        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }
}
