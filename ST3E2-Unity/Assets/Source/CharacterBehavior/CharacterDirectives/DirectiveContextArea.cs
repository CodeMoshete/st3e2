using System;
using System.Collections.Generic;

[Serializable]
public class DirectiveContextArea
{
    public string Name;
    public int EntryWeight;
    public int ExitWeight;
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
    public string NavNetworkName;
    public bool DirectivesInheritBaseSettings;
    public List<CharacterDirective> Directives;
}
