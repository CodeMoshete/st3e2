public class SetNpcComponentsActiveAction : NpcBaseAction
{
    public bool OverrideEnableView;
    public bool OverrideDisableView;
    public bool IsNavigationActive;
    public bool IsDirectivesActive;
    public bool IsConversationsActive;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        // TODO - Is this logic working as intended?
        if (OverrideEnableView && !TargetEntity.ShowView)
        {
            TargetEntity.LoadAndShowView();
        }
        else if (OverrideDisableView && TargetEntity.ShowView)
        {
            TargetEntity.DestroyView();
        }

        TargetEntity.NavComponent.IsNavigationEnabled = IsNavigationActive;
        TargetEntity.DirectiveComponent.IsDirectivesEnabled = IsDirectivesActive;
        TargetEntity.ConversationComponent.IsConversationsActive = IsConversationsActive;

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
