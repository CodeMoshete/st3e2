﻿public static class Service
{
    private static EventManager eventManager;
    public static EventManager EventManager
    {
        get
        {
            if (eventManager == null)
            {
                eventManager = new EventManager();
            }

            return eventManager;
        }
    }

    private static TimerManager timerMananager;
    public static TimerManager TimerManager
    {
        get
        {
            if (timerMananager == null)
            {
                timerMananager = new TimerManager();
            }

            return timerMananager;
        }
    }

    private static CharacterSystemManager characterSystems;
    public static CharacterSystemManager CharacterSystems
    {
        get
        {
            if (characterSystems == null)
            {
                characterSystems = new CharacterSystemManager();
            }

            return characterSystems;
        }
    }

    private static TimeOfDayService timeOfDay;
    public static TimeOfDayService TimeOfDay
    {
        get
        {
            if (timeOfDay == null)
            {
                timeOfDay = new TimeOfDayService();
            }

            return timeOfDay;
        }
    }

    public static NavWorldManager NavWorldManager
    {
        get
        {
            return NavWorldManager.Instance;
        }
    }

    // Manually set services
    public static PlayerData PlayerData
    {
        get
        {
            return PlayerData.Instance;
        }
    }

    public static ControlsManager Controls
    {
        get
        {
            return ControlsManager.Instance;
        }
    }

    public static UpdateManager UpdateManager
    {
        get
        {
            return UpdateManager.Instance;
        }
    }

    public static WorldActionsManager SceneActions
    {
        get
        {
            return WorldActionsManager.Instance;
        }
    }
}
