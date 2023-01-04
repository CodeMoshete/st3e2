public class SetNpcAnimationState : NpcBaseAction
{
    public string StateName;
    public bool StateValue;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        TargetEntity.AnimComponent.SetBool(StateName, StateValue);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
