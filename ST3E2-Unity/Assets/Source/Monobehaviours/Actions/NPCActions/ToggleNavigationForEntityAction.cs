using UnityEngine;

public class ToggleNavigationForEntityAction : NpcBaseAction
{
    public bool IsNavigationActive;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        TargetEntity.NavComponent.IsNavigating = IsNavigationActive;
        Debug.Log(TargetEntity.name + " IsNavigating set to " + TargetEntity.NavComponent.IsNavigating);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
