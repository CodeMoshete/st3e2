public class HideObjectiveIconAction : CustomAction
{
    public ShowObjectiveIconAction ShowAction;
    public CustomAction NextAction;

    public override void Initiate()
    {
        ShowAction.HideObjectiveIcon();

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
