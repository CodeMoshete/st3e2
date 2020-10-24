public class SetControlSchemeAction : CustomAction
{
    public ControlScheme NewControlScheme;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        Service.EventManager.SendEvent(EventId.SetNewControlScheme, NewControlScheme);

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
