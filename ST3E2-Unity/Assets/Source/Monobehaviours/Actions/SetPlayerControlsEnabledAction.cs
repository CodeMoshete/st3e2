public class SetPlayerControlsEnabledAction : CustomAction
{
    public bool TouchEnabled;
    public bool TriggerEnabled;
    public bool BackButtonEnabled;

    public bool PlayerMovementEnabled;

    public CustomAction OnComplete;

    public override void Initiate()
    {
        Service.Controls.DisableTouchInput = !TouchEnabled;
        Service.Controls.DisableTriggerInput = !TriggerEnabled;
        Service.Controls.DisableBackButtonInput = !BackButtonEnabled;
        Service.EventManager.SendEvent(EventId.SetControlsEnabled, PlayerMovementEnabled);

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
