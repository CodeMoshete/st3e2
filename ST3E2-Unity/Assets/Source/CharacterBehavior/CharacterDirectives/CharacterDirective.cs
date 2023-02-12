using System;

[Serializable]
public class CharacterDirective
{
    public bool TimeOfDayBound;
    public string StartTime;
    private float startTimeSeconds = -1f;
    public float StartTimeSeconds
    {
        get
        {
            if (startTimeSeconds < 0f)
            {
                startTimeSeconds = Service.TimeOfDay.FormattedTimeToSeconds(StartTime);
            }
            return startTimeSeconds;
        }
    }

    public string EndTime;
    private float endTimeSeconds = -1f;
    public float EndTimeSeconds
    {
        get
        {
            if (endTimeSeconds < 0f)
            {
                endTimeSeconds = Service.TimeOfDay.FormattedTimeToSeconds(EndTime);
            }
            return endTimeSeconds;
        }
    }

    public float MinDuration = 15f;
    public float MaxDuration = 120f;
    public float DirectiveWeight;
    public string NavNetworkName;
    public string NavNodeName;
}
