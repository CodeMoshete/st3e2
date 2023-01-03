public class ToggleNavigationForEntityAction : NpcBaseAction
{
    public bool IsNavigationActive;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        TargetEntity.NavComponent.IsNavigating = IsNavigationActive;

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
