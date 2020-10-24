using UnityEngine;
using Utils;

public class TeleportControlScheme : IControlScheme
{
    private const string RIGHT_CONTROLLER_NAME = "RightControllerAnchor";
    private const string TELEPORT_MARKER = "TeleportMarker";
    private const string TELEPORT_MARKER_OBJ = "TeleportMarkerObj";
    private const string TELEPORT_TAG = "Teleport";
    private const string LAYER_AREA_TRIGGER = "AreaTrigger";
    private const string LAYER_BACKGROUND = "Background";
    private const string TAG_AREA_LOCK = "AreaLock";
    private const float SPHERECAST_RADIUS = 0.2f;
    private const float PLAYER_HEIGHT = 0.9f;
    private const float TELEPORT_TIME = 0.25f;
    private const float RAYCAST_DIST = 50f;
    private const float MAX_TELEPORT_SQR_DIST = 16f;
    private readonly Color COLOR_TELEPORT_OK = Color.white;
    private readonly Color COLOR_TELEPORT_BLOCKED = Color.red;
    private readonly Color COLOR_TELEPORT_ACTION_LOC = Color.green;

    private bool disableMovement;
    private bool disableMovementForChoice;
    private bool wasMovementDisabledForChoice;

    private float playerHeightOculusGo
    {
        get
        {
            return Engine.OCULUS_GO_HEIGHT * 2f;
        }
    }

    private OVRPlayerController bodyObject;
    private Transform cameraObject;
    private OVRScreenFade screenFader;
    private Transform rightController;
    private Vector3 lastMousePos;

    private float sensitivity;
    private bool isDebugMenuActive;

    private GameObject teleportMarker;
    private Material teleportMaterial;
    private float currentTeleportTime;
    private Vector3 targetTeleportPos;
    private int raycastLayerMask;
    private bool isTeleportingOut;
    private bool isTeleporting;

    private Transform camController;
    private Transform camRig;
    private Transform cameraEye;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        bodyObject = body;
        cameraObject = camera;
        camController = camera.parent;
        this.sensitivity = sensitivity;

        rightController = UnityUtils.FindGameObject(bodyObject.gameObject, RIGHT_CONTROLLER_NAME).transform;
        GameObject centerEye = UnityUtils.FindGameObject(cameraObject.gameObject, "CenterEyeAnchor");
        screenFader = centerEye.GetComponent<OVRScreenFade>();
        cameraEye = centerEye.transform;

        teleportMarker = GameObject.Instantiate(Resources.Load<GameObject>(TELEPORT_MARKER));
        teleportMarker.SetActive(false);
        GameObject teleportMarkerInner = UnityUtils.FindGameObject(teleportMarker, TELEPORT_MARKER_OBJ);
        teleportMaterial = teleportMarkerInner.GetComponent<Renderer>().sharedMaterial;

        raycastLayerMask = ~(LayerMask.GetMask(LAYER_AREA_TRIGGER) | LayerMask.GetMask(LAYER_BACKGROUND));

        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);

        Service.EventManager.AddListener(EventId.ShowChoiceDialogue, ChoiceDialogueShown);
        Service.EventManager.AddListener(EventId.ChoiceDialogueDismissed, ChoiceDialogueHidden);
    }

    public void Deactivate()
    {
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
        Service.EventManager.RemoveListener(EventId.ShowChoiceDialogue, ChoiceDialogueShown);
        Service.EventManager.RemoveListener(EventId.ChoiceDialogueDismissed, ChoiceDialogueHidden);
    }

    public bool ChoiceDialogueShown(object cookie)
    {
        disableMovementForChoice = true;
        wasMovementDisabledForChoice = true;
        teleportMarker.SetActive(false);
        return false;
    }

    public bool ChoiceDialogueHidden(object cookie)
    {
        disableMovementForChoice = false;
        return false;
    }

    public void SetMovementEnabled(bool enabled)
    {
        Debug.Log("Controls active: " + enabled);
        disableMovement = !enabled;
        teleportMarker.SetActive(false);
    }

    private void BackUpdate(BackButtonUpdate update)
    {
        if (update.BackButtonClicked)
        {
            isDebugMenuActive = !isDebugMenuActive;
            Service.EventManager.SendEvent(EventId.DebugToggleConsole, isDebugMenuActive);
        }
    }

    private void TouchUpdate(TouchpadUpdate update)
    {
#if UNITY_EDITOR
        Vector3 euler = bodyObject.transform.eulerAngles;
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            float delta = Time.deltaTime;
            Vector3 mouseDelta = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            euler = bodyObject.transform.eulerAngles;
            euler.y += delta * -((mouseDelta.x / Screen.width) * sensitivity);
            bodyObject.transform.eulerAngles = euler;

            euler = cameraObject.eulerAngles;
            euler.x += delta * (mouseDelta.y / Screen.height) * sensitivity;
            cameraObject.eulerAngles = euler;
        }
#endif

        if (isTeleporting || disableMovement || disableMovementForChoice)
            return;

        float dt = Time.deltaTime;

        bool isPressed = update.TouchpadPressState;
        bool isPressedThisFrame = update.TouchpadClicked;

        if (ControlsManager.Instance.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            OVRInput.Controller activeController = OVRInput.GetActiveController();
        }

        Ray testRay = new Ray(rightController.position, rightController.forward);
#if UNITY_EDITOR
        isPressed = Input.GetKey(KeyCode.T);
        testRay = new Ray(cameraEye.position, cameraEye.forward);
#endif
        if (isPressed && !wasMovementDisabledForChoice)
        {
            RaycastHit hit;
            if (Physics.SphereCast(testRay, SPHERECAST_RADIUS, out hit, RAYCAST_DIST, raycastLayerMask))
            {
                if (hit.collider.tag == TELEPORT_TAG)
                {
                    float sqrDist =
                        Vector3.SqrMagnitude(teleportMarker.transform.position - bodyObject.transform.position);

                    teleportMaterial.color = sqrDist > MAX_TELEPORT_SQR_DIST ?
                        COLOR_TELEPORT_BLOCKED :
                        COLOR_TELEPORT_OK;

                    teleportMarker.transform.position = hit.point;
                    teleportMarker.SetActive(true);
                }
                else if(hit.collider.tag == TAG_AREA_LOCK)
                {
                    Ray floorTestRay = new Ray(hit.collider.transform.position, -hit.collider.transform.up);
                    RaycastHit floorHit;
                    if (Physics.Raycast(floorTestRay, out floorHit, RAYCAST_DIST, raycastLayerMask))
                    {
                        teleportMarker.transform.position = floorHit.point;

                        float sqrDist =
                        Vector3.SqrMagnitude(teleportMarker.transform.position - bodyObject.transform.position);

                        teleportMaterial.color = sqrDist > MAX_TELEPORT_SQR_DIST ?
                            COLOR_TELEPORT_BLOCKED :
                            COLOR_TELEPORT_ACTION_LOC;

                        teleportMarker.SetActive(true);
                    }
                }
                else
                {
                    Debug.Log(hit.collider.gameObject.name);
                    teleportMarker.SetActive(false);
                }
            }
        }
        else if (teleportMarker.activeInHierarchy)
        {
            teleportMarker.SetActive(false);

            float sqrDist = 
                Vector3.SqrMagnitude(teleportMarker.transform.position - bodyObject.transform.position);

            if (sqrDist <= MAX_TELEPORT_SQR_DIST)
            {
                isTeleporting = true;
                Vector3 teleportPos = teleportMarker.transform.position;
                float originalHeight = teleportPos.y;
                float finalHeight = 
                    Service.Controls.CurrentHeadset == HeadsetModel.OculusGo ? playerHeightOculusGo : PLAYER_HEIGHT;
                teleportPos.y += finalHeight;
                TeleportTo(teleportPos);

                //Transform centerEye = UnityUtils.FindGameObject(cameraObject.gameObject, "CenterEyeAnchor").transform;
                //Transform camRig = cameraObject.parent.transform;
                //string message =
                //    "FinalHeight: " + finalHeight +
                //    "\nTeleportPos: " + teleportPos.y +
                //    "\nOriginalHeight: " + originalHeight +
                //    "\nCenterEye: " + centerEye.localPosition.y +
                //    "\nCamRig: " + camRig.localPosition.y +
                //    "\nBody: " + bodyObject.transform.position.y;
                //Service.EventManager.SendEvent(EventId.LogDebugMessage, message);
            }
        }

        if (wasMovementDisabledForChoice && !isPressed)
        {
            wasMovementDisabledForChoice = false;
        }
    }

    private void TeleportTo(Vector3 position)
    {
        currentTeleportTime = TELEPORT_TIME;
        targetTeleportPos = position;
        isTeleportingOut = true;
        Service.UpdateManager.AddObserver(Update);
    }

    private void Update(float dt)
    {
        currentTeleportTime = Mathf.Max(0f, currentTeleportTime - dt);

        float pct = isTeleportingOut ? 
            1f - (currentTeleportTime / TELEPORT_TIME) :
            currentTeleportTime / TELEPORT_TIME;

        screenFader.SetFadeLevel(pct);

        if (currentTeleportTime == 0f)
        {
            if (isTeleportingOut)
            {
                bodyObject.transform.position = targetTeleportPos;

                if (Service.Controls.CurrentHeadset == HeadsetModel.OculusGo)
                {
                    cameraEye.localPosition = Vector3.zero;
                    Vector3 camRigPos = camController.localPosition;
                    camRigPos.y = Engine.OCULUS_GO_HEIGHT;
                    camController.localPosition = camRigPos;
                }

                currentTeleportTime = TELEPORT_TIME;
                isTeleportingOut = false;
            }
            else
            {
                Service.UpdateManager.RemoveObserver(Update);
                isTeleporting = false;
            }
        }
    }
}
