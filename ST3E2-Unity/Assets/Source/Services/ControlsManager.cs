using System;
using System.Collections.Generic;
using UnityEngine;

public enum HeadsetModel
{
    None,
    OculusGo,
    OculusQuest
}

public enum ControlsType
{
    Trigger,
    Touchpad,
    BackButton
}

public struct TouchpadUpdate
{
    public Vector2 TouchpadPosition;
    public bool TouchpadPressState;
    public bool TouchpadClicked;
}

public struct TriggerUpdate
{
    public bool TriggerPressState;
    public bool TriggerClicked;
}

public struct BackButtonUpdate
{
    public bool BackButtonPressState;
    public bool BackButtonClicked;
}

public class ControlsManager
{
    private List<Action<TouchpadUpdate>> touchpadListeners;
    private List<Action<TriggerUpdate>> triggerListeners;
    private List<Action<BackButtonUpdate>> backButtonListeners;

    [HideInInspector]
    public bool DisableTouchInput;
    [HideInInspector]
    public bool DisableTriggerInput;
    [HideInInspector]
    public bool DisableBackButtonInput;

    public HeadsetModel CurrentHeadset;

    private static ControlsManager instance;
    public static ControlsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ControlsManager();
            }
            return instance;
        }
    }

    public ControlsManager()
    {
        string deviceModel = SystemInfo.deviceModel;
        CurrentHeadset = HeadsetModel.None;
        switch (deviceModel)
        {
            case Constants.DEVICE_TYPE_GO:
                CurrentHeadset = HeadsetModel.OculusGo;
                break;
            case Constants.DEVICE_TYPE_QUEST:
                CurrentHeadset = HeadsetModel.OculusQuest;
                break;
        }

        touchpadListeners = new List<Action<TouchpadUpdate>>();
        triggerListeners = new List<Action<TriggerUpdate>>();
        backButtonListeners = new List<Action<BackButtonUpdate>>();
        Service.UpdateManager.AddObserver(Update);
    }

    private void Update(float dt)
    {
        OVRInput.Controller activeController = OVRInput.GetActiveController();

        if (!DisableTouchInput && touchpadListeners.Count > 0)
        {
            TouchpadUpdate touchUpdate = new TouchpadUpdate();
#if UNITY_EDITOR
            touchUpdate.TouchpadPosition = new Vector2(0f, 0f);
            if (Input.GetKey(KeyCode.W))
            {
                touchUpdate.TouchpadPosition.y += 0.5f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                touchUpdate.TouchpadPosition.y -= 0.5f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                touchUpdate.TouchpadPosition.x += 0.5f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                touchUpdate.TouchpadPosition.x -= 0.5f;
            }

            touchUpdate.TouchpadClicked =
                Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D);

            touchUpdate.TouchpadPressState =
                Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D);
#else

            if (CurrentHeadset == HeadsetModel.OculusGo)
            {
                touchUpdate.TouchpadPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                touchUpdate.TouchpadPressState = true;

                if (touchUpdate.TouchpadPosition == Vector2.zero)
                {
                    touchUpdate.TouchpadPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                    touchUpdate.TouchpadPressState = OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
                    touchUpdate.TouchpadClicked = OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad);
                }
            }
            else if (CurrentHeadset == HeadsetModel.OculusQuest)
            {
                touchUpdate.TouchpadPosition = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, activeController);
                touchUpdate.TouchpadPressState = touchUpdate.TouchpadPosition.sqrMagnitude > 0.3f;
            }
#endif
            touchpadListeners[touchpadListeners.Count - 1](touchUpdate);
        }

        if (!DisableTriggerInput && triggerListeners.Count > 0)
        {
            TriggerUpdate triggerUpdate = new TriggerUpdate();
#if UNITY_EDITOR
            triggerUpdate.TriggerClicked = Input.GetKeyDown(KeyCode.I);
            triggerUpdate.TriggerPressState = Input.GetKey(KeyCode.I);
#else
            if (CurrentHeadset == HeadsetModel.OculusGo)
            {
                triggerUpdate.TriggerPressState = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
                triggerUpdate.TriggerClicked = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
            }
            else if (CurrentHeadset == HeadsetModel.OculusQuest)
            {
                triggerUpdate.TriggerPressState = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
                triggerUpdate.TriggerClicked = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
            }
#endif
            triggerListeners[triggerListeners.Count - 1](triggerUpdate);
        }

        if (!DisableBackButtonInput && backButtonListeners.Count > 0)
        {
            BackButtonUpdate backButtonUpdate = new BackButtonUpdate();
#if UNITY_EDITOR
            backButtonUpdate.BackButtonClicked = Input.GetKeyDown(KeyCode.Escape);
            backButtonUpdate.BackButtonPressState = Input.GetKey(KeyCode.Escape);
#else
            if (CurrentHeadset == HeadsetModel.OculusGo)
            {
                backButtonUpdate.BackButtonPressState = OVRInput.Get(OVRInput.Button.Back);
                backButtonUpdate.BackButtonClicked = OVRInput.GetDown(OVRInput.Button.Back);
            }
            else if (CurrentHeadset == HeadsetModel.OculusQuest)
            {
                backButtonUpdate.BackButtonClicked = OVRInput.GetDown(OVRInput.RawButton.B);
                backButtonUpdate.BackButtonPressState = OVRInput.Get(OVRInput.RawButton.B);
            }
#endif
            backButtonListeners[backButtonListeners.Count - 1](backButtonUpdate);
        }
    }

    public void SetTouchObserver(Action<TouchpadUpdate> callback)
    {
        touchpadListeners.Add(callback);
    }

    public void RemoveTouchObserver(Action<TouchpadUpdate> callback)
    {
        touchpadListeners.Remove(callback);
    }

    public void SetTriggerObserver(Action<TriggerUpdate> callback)
    {
        triggerListeners.Add(callback);
    }

    public void RemoveTriggerObserver(Action<TriggerUpdate> callback)
    {
        triggerListeners.Remove(callback);
    }

    public void SetBackButtonObserver(Action<BackButtonUpdate> callback)
    {
        backButtonListeners.Add(callback);
    }

    public void RemoveBackButtonObserver(Action<BackButtonUpdate> callback)
    {
        backButtonListeners.Remove(callback);
    }
}
