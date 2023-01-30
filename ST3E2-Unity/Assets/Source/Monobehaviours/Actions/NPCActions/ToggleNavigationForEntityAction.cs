using UnityEngine;

public class ToggleNavigationForEntityAction : NpcBaseAction
{
    public bool IsNavigationActive;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        TargetEntity.NavComponent.IsNavigationEnabled = IsNavigationActive;
        Debug.Log(TargetEntity.name + " IsNavigating set to " + TargetEntity.NavComponent.IsNavigationEnabled);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
