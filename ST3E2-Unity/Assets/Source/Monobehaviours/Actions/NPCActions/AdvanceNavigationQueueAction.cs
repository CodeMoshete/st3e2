public class AdvanceNavigationQueueAction : NpcBaseAction
{
    public int NumberToAdvance = 1;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        for (int i = 0; i < NumberToAdvance; ++i)
        {
            TargetEntity.NavComponent.NavigationQueue.Dequeue();
        }

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
