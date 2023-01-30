using System;

[Serializable]
public class CharacterDirective
{
    public bool TimeOfDayBound;
    public float StartTime;
    public float EndTime;
    public float MinDuration = 15f;
    public float MaxDuration = 120f;
    public float DirectiveWeight;
    public string NavNetworkName;
    public string NavNodeName;
}
