public class AdvanceNavigationQueueAction : NpcBaseAction
{
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        TargetEntity.NavComponent.NavigationQueue.Dequeue();

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
