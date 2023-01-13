using System;

[Serializable]
public class CharacterDirective
{
    public bool TimeOfDayBound;
    public float StartTime;
    public float EndTime;
    public float MinDuration;
    public float MaxDuration;
    public float DirectiveWeight;
    public string NavNetworkName;
    public string NavNodeName;
}
