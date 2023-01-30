using UnityEngine;

public class TimeOfDayService
{
    // 12 in-game minutes pass every minute.
    // This equates to one full in-game day every 2 hours.
    private const float TIME_MULTIPLIER = 12f;
    private const float SECS_IN_DAY = 86400;
    private const string ERROR_LOG = "Invalid format! Should be 00:00, but got {0}";

    // For debug purposes only.
    private int lastSeenMinute = -1;

    public float CurrentDaySeconds { get; private set; }
    public string CurrentFormattedTime
    {
        get
        {
            int numHours = Mathf.FloorToInt(CurrentDaySeconds / 3600f);
            int numMinutes = Mathf.FloorToInt(CurrentDaySeconds / 60f) % (numHours * 3600);
            return string.Format("{0}:{1}", numHours, numMinutes);
        }

        set
        {
            CurrentDaySeconds = FormattedTimeToSeconds(value);
            Debug.Log("Time of day set to " + CurrentDaySeconds);
        }
    }

    public bool IsPaused { get; private set; }

    public TimeOfDayService()
    {
        Service.UpdateManager.AddObserver(OnUpdate);
    }

    private void OnUpdate(float dt)
    {
        if (IsPaused)
        {
            return;
        }

        CurrentDaySeconds += dt * 12f;
        if (CurrentDaySeconds > SECS_IN_DAY)
        {
            // A new day has begun.
            CurrentDaySeconds = CurrentDaySeconds % SECS_IN_DAY;
        }

        int currentMinute = Mathf.FloorToInt(CurrentDaySeconds / 60f);
        if (currentMinute != lastSeenMinute)
        {
            lastSeenMinute = currentMinute;
            Debug.Log("Current time: " + string.Format(CurrentSecondsToFormattedTime()));
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Unpause()
    {
        IsPaused = false;
    }

    public float FormattedTimeToSeconds(string formattedTime)
    {
        string[] parts = formattedTime.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogError(string.Format(ERROR_LOG, formattedTime));
            return 0f;
        }

        int numHours = 0;
        bool hoursParsable = int.TryParse(parts[0], out numHours);

        int numMins = 0;
        bool minsParsable = int.TryParse(parts[1], out numMins);

        if (!hoursParsable || !minsParsable)
        {
            Debug.LogError(string.Format(ERROR_LOG, formattedTime));
            return 0f;
        }

        return (float)numHours * 3600f + (float)numMins * 60f;
    }

    public string CurrentSecondsToFormattedTime()
    {
        int hour = Mathf.FloorToInt(CurrentDaySeconds / 3600);
        string hourContent = hour < 10 ? "0" + hour : hour.ToString();
        int minute = Mathf.FloorToInt(CurrentDaySeconds % 3600) / 60;
        string minuteContent = minute < 10 ? "0" + minute : minute.ToString();
        return string.Format("{0}:{1}", hourContent, minuteContent);
    }
}
