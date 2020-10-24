using UnityEngine;

public class SendStringEvent : CustomAction
{
    public EventId Event;
    public string Value;

    public override void Initiate()
    {
        Service.EventManager.SendEvent(Event, Value);
    }
}
