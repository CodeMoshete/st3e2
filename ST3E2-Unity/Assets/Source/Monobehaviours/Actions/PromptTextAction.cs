public class PromptTextAction : CustomAction
{
    public string PromptText;
    public float Duration;
    public CustomAction OnDone;

    public override void Initiate()
    {
        PromptTextActionData evtData = new PromptTextActionData(PromptText, Duration);
        Service.EventManager.SendEvent(EventId.ShowPromptText, evtData);

        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }
}
