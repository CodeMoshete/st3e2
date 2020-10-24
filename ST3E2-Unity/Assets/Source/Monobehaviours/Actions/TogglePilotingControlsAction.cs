public class TogglePilotingControlsAction : CustomAction
{
    public bool UseTargeting;
    public bool UseSteering;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        Service.EventManager.SendEvent(EventId.TogglePilotingControls, this);

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
