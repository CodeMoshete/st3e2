public class SetNpcComponentsActiveAction : NpcBaseAction
{
    public bool IsViewActive;
    public bool IsNavigationActive;
    public bool IsDirectivesActive;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        if (IsViewActive && !TargetEntity.ShowView)
        {
            TargetEntity.DestroyView();
        }
        else if (!IsViewActive && TargetEntity.ShowView)
        {
            TargetEntity.LoadAndShowView();
        }

        TargetEntity.NavComponent.IsNavigationEnabled = IsNavigationActive;
        TargetEntity.DirectiveComponent.IsDirectivesEnabled = IsDirectivesActive;

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
