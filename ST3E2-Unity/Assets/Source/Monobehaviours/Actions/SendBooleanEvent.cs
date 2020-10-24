using UnityEngine;

public class SendBooleanEvent : CustomAction
{
    public EventId Event;
    public bool Value;
    public CustomAction OnDone;

    public override void Initiate()
    {
        Service.EventManager.SendEvent(Event, Value);

        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }
}
