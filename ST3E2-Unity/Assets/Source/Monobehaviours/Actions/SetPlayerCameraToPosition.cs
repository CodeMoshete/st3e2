using UnityEngine;
using Utils;

public class SetPlayerCameraToPosition : CustomAction
{
    public Transform ReferenceHeightObject;
    public Transform OVRPlayerController;
    public CustomAction OnDone;
    private bool displayExistedLastFrame;

    public override void Initiate()
    {
#if UNITY_EDITOR
        RepositionDisplay();
#else
        if (ReferenceHeightObject != null)
        {
            Service.UpdateManager.AddObserver(OnUpdate);
        }
#endif
        if (OnDone != null)
        {
            OnDone.Initiate();
        }
    }

    private void RepositionDisplay()
    {
        Vector3 cameraPos =
            UnityUtils.FindGameObject(OVRPlayerController.gameObject, "CenterEyeAnchor").transform.position - 
            OVRPlayerController.position;

        Vector3 targetPos = ReferenceHeightObject.position;
        targetPos.y -= cameraPos.y;
        OVRPlayerController.position = targetPos;
    }

    private void OnUpdate(float dt)
    {
        if (displayExistedLastFrame)
        {
            RepositionDisplay();
            Service.UpdateManager.RemoveObserver(OnUpdate);
        }

        displayExistedLastFrame = OVRManager.display != null;
    }

}
