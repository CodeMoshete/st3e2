using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Engine : MonoBehaviour
{
    public const float OCULUS_GO_HEIGHT = 0.5f;
    public const float OCULUS_GO_SCALE = 1.25f;

    [System.Serializable]
    public struct PlayerCameraMirror
    {
        public Transform MirrorCamera;
        public float MovementScale;
    }

    public float BackgroundCameraScale = 1f;

    public List<PlayerCameraMirror> MirrorCameras;

    public Text ConsoleLine;
	public float Sensitivity = 50f;

    public Transform HeadCamera;

    private Dictionary<ControlScheme, IControlScheme> controlSchemes;
    private IControlScheme currentControlScheme;

    private Transform backgroundCamera;
	private OVRPlayerController bodyObject;
	private Transform cameraObject;
    private Transform eyeObject;
    private bool isDebugMenuActive;

    private Vector3 lastPlayerPosition;

    private bool isJobsActive = true;

	void Start ()
    {
		bodyObject = GameObject.Find ("OVRPlayerController").GetComponent<OVRPlayerController>();
        for (int i = 0, count = MirrorCameras.Count; i < count; ++i)
        {
            float scale = MirrorCameras[i].MovementScale;
            MirrorCameras[i].MirrorCamera.localScale = new Vector3(scale, scale, scale);
        }

		cameraObject = UnityUtils.FindGameObject(bodyObject.gameObject, "TrackingSpace").transform;
        eyeObject = UnityUtils.FindGameObject(bodyObject.gameObject, "CenterEyeAnchor").transform;
        lastPlayerPosition = cameraObject.transform.position;

        UpdateManager manager = UpdateManager.Instance;

        controlSchemes = new Dictionary<ControlScheme, IControlScheme>();
        controlSchemes.Add(ControlScheme.Movement, new MovementControlScheme());
        controlSchemes.Add(ControlScheme.Teleport, new TeleportControlScheme());
        controlSchemes.Add(ControlScheme.Piloting, new PilotingControlScheme());
        SetControlScheme(ControlScheme.Teleport);

        Service.EventManager.AddListener(EventId.SetControlsEnabled, OnControlsEnableSet);
        Service.EventManager.AddListener(EventId.SetNewControlScheme, OnControlSchemeChanged);
        Service.EventManager.AddListener(EventId.LogDebugMessage, OnLogDebugMessage);

        if (Service.Controls.CurrentHeadset == HeadsetModel.OculusGo)
        {
            ForceOculusGoCameraHeight();
        }

        //Log("Device name: " + SystemInfo.deviceName + 
        //    "\nDevice model: " + SystemInfo.deviceModel +
        //    "\nDevice Type: " + SystemInfo.deviceType +
        //Log("eyPos: " + eyeObject.localPosition.ToString() +
        //    "\nTrackingPos: " + cameraObject.localPosition.ToString() +
        //    "\nCamPos: " + HeadCamera.localPosition.ToString() +
        //    "\nTracking Type: " + ovrManager.trackingOriginType.ToString());
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.SetControlsEnabled, OnControlsEnableSet);
        Service.EventManager.RemoveListener(EventId.SetNewControlScheme, OnControlSchemeChanged);
        Service.EventManager.RemoveListener(EventId.LogDebugMessage, OnLogDebugMessage);
        currentControlScheme.Deactivate();
    }

    private void ForceOculusGoCameraHeight()
    {
        OVRManager ovrManager = HeadCamera.GetComponent<OVRManager>();
        Vector3 cameraStartPos = HeadCamera.localPosition;
        bodyObject.useProfileData = false;
        bodyObject.transform.localScale = new Vector3(OCULUS_GO_SCALE, OCULUS_GO_SCALE, OCULUS_GO_SCALE);
        ovrManager.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
        cameraStartPos.y = OCULUS_GO_HEIGHT;
        HeadCamera.localPosition = cameraStartPos;
    }

    private void SetControlScheme(ControlScheme type)
    {
        IControlScheme nextScheme = controlSchemes[type];
        if (nextScheme != currentControlScheme && currentControlScheme != null)
        {
            currentControlScheme.Deactivate();
        }
        currentControlScheme = nextScheme;
        currentControlScheme.Initialize(bodyObject, cameraObject, Sensitivity);
    }

    public bool OnControlsEnableSet(object cookie)
    {
        currentControlScheme.SetMovementEnabled((bool)cookie);
        return true;
    }

    private bool OnControlSchemeChanged(object cookie)
    {
        SetControlScheme((ControlScheme)cookie);
        return false;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.U))
        {
            isDebugMenuActive = !isDebugMenuActive;
            Service.EventManager.SendEvent(EventId.DebugToggleConsole, isDebugMenuActive);
        }

        if (isDebugMenuActive && Input.GetKeyDown(KeyCode.J))
        {
            isJobsActive = !isJobsActive;
            Log("Jobs activated: " + isJobsActive);
            Service.EventManager.SendEvent(EventId.DebugToggleJobs, isJobsActive);
        }

        for (int i = 0, count = MirrorCameras.Count; i < count; ++i)
        {
            float scale = MirrorCameras[i].MovementScale;
            Vector3 moveDist = (cameraObject.transform.position - lastPlayerPosition) * scale;
            MirrorCameras[i].MirrorCamera.localPosition += moveDist;
            MirrorCameras[i].MirrorCamera.localRotation = cameraObject.transform.rotation;
        }
        lastPlayerPosition = cameraObject.transform.position;
    }

    private void LateUpdate()
    {
        //Log("Body: " + bodyObject.transform.localEulerAngles.ToString() + "\n" +
        //    "Tracking Space: " + cameraObject.localEulerAngles.ToString() + "\n" +
        //    "Head: " + HeadCamera.localEulerAngles.ToString() + "\n" +
        //    "Eyes: " + eyeObject.localEulerAngles.ToString());

    //    if (Service.Controls.CurrentHeadset == HeadsetModel.OculusGo &&
    //        HeadCamera.localPosition.y != OCULUS_GO_HEIGHT)
    //    {
    //        ForceOculusGoCameraHeight();
    //    }
    }

    private bool OnLogDebugMessage(object cookie)
    {
        Log((string)cookie);
        return true;
    }

    private void Log(string message)
    {
        if (ConsoleLine != null)
        {
            ConsoleLine.text = message;
        }
        Debug.Log(message);
    }
}
