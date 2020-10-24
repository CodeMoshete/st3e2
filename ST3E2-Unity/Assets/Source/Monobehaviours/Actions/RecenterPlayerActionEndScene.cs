using UnityEngine;

public class RecenterPlayerActionEndScene : CustomAction
{
    public Transform OVRPlayerController;
    public CustomAction OnDone;
    public float CompletionDelay;
    public float Rotation;

    public override void Initiate()
    {
        Debug.Log("Set player rotation PRE: " + OVRPlayerController.localEulerAngles.ToString());
        OVRPlayerController.localEulerAngles = new Vector3(0f, Rotation, 0f);
        Debug.Log("Set player rotation POST: " + OVRPlayerController.localEulerAngles.ToString());

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
}
