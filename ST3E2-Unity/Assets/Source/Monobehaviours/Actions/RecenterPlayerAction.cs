using System.Collections;
using UnityEngine;
using Utils;

public class RecenterPlayerAction : CustomAction
{
    public Transform OVRPlayerController;
    public CustomAction OnDone;
    public CustomAction OnPlayerRecentered;
    public float CompletionDelay;
    public float Rotation;
    private Transform centerEyeAnchor;
    private Vector3 initialRotation;
    private bool displayExistedLastFrame;
    private bool wasCameraUpdated;

    public override void Initiate()
    {
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
        initialRotation = OVRPlayerController.localEulerAngles;

#if UNITY_EDITOR
        RecenterDisplay();
        wasCameraUpdated = true;
#else
        OVRPlayerController.GetComponent<OVRPlayerController>().CameraUpdated += OnCameraUpdated;
        Service.UpdateManager.AddObserver(OnUpdate);
#endif
        if (CompletionDelay <= 0f)
        {
            ExecuteCallback(null);
        }
        else
        {
            Service.TimerManager.CreateTimer(CompletionDelay, ExecuteCallback, null);
        }
    }

    private void ExecuteCallback(object cookie)
    {
        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }

    private void OnCameraUpdated()
    {
        wasCameraUpdated = true;
    }

    private void RecenterDisplay()
    {
        StartCoroutine(ResetCamera());
    }

    IEnumerator ResetCamera()
    {
        while (OVRPlayerController.localEulerAngles == initialRotation && !wasCameraUpdated)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        Debug.Log("Camera position reset PRE: " + OVRPlayerController.localEulerAngles.ToString());
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
        Debug.Log("Camera position reset POST: " + OVRPlayerController.localEulerAngles.ToString());

        if (OnPlayerRecentered != null)
        {
            OnPlayerRecentered.Initiate();
        }
    }

    private void OnUpdate(float dt)
    {
        if (displayExistedLastFrame)
        {
            RecenterDisplay();
            Service.UpdateManager.RemoveObserver(OnUpdate);
        }

        displayExistedLastFrame = OVRManager.display != null;
    }
}
