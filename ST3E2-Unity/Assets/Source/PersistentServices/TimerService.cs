using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerService : MonoBehaviour
{
    public static TimerService Instance { get; private set; }

    private class TimerInstance
    {
        public float TimeLeft;
        public Action<object> Callback;
        public Action<object> UpdateCallback;
        public object Cookie;
    }

    private List<TimerInstance> timers;
    private Stack<TimerInstance> timersToRemove;

    public TimerService()
    {
        timers = new List<TimerInstance>();
        timersToRemove = new Stack<TimerInstance>();
    }

    public void Start()
    {
        Instance = this;
    }

    public void CreateTimer(
        float duration, Action<object> onUpdate, Action<object> onComplete, object cookie)
    {
        TimerInstance timer = new TimerInstance
        {
            TimeLeft = duration,
            Callback = onComplete,
            UpdateCallback = onUpdate,
            Cookie = cookie
        };

        timers.Add(timer);
    }

    public void CreateTimer(float duration, Action<object> callback, object cookie)
    {
        TimerInstance timer = 
            new TimerInstance { TimeLeft = duration, Callback = callback, Cookie = cookie };
        timers.Add(timer);
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0, count = timers.Count; i < count; ++i)
        {
            TimerInstance timer = timers[i];
            timer.TimeLeft -= dt;
            if (timer.TimeLeft <= 0)
            {
                timer.Callback.Invoke(timer.Cookie);
                timersToRemove.Push(timer);
            }
            else if (timer.UpdateCallback != null)
            {
                timer.UpdateCallback(timer.Cookie);
            }
        }

        while(timersToRemove.Count > 0)
        {
            timers.Remove(timersToRemove.Pop());
        }
    }
}
