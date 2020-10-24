using UnityEngine;
using UnityEngine.UI;
using Utils;

public class PilotingControlScheme : IControlScheme
{
    private const string PILOTING_CONTAINER_NAME = "PilotingContainer";
    private const string PILOTING_CAMERA_NAME = "CenterEyeAnchor";
    private const string WEAPON_NAME = "Weapon";
    private const string LAYER_SHOOTABLE = "BGShootable";
    private const string BG_HUD_CANVAS = "BackgroundHUDCanvas";
    private const string RETICLE_NAME = "Reticle";
    private const float ACCELERATION = 0.01f;
    private const float DRAG = 0.92f;
    private const float MAX_SPEED_SQR = 0.0225f;
    private const float MAX_SPEED = 0.15f;
    private const float MAX_DIST_FROM_CENTER_SQR = 0.0225f;
    private const float MAX_CAM_TILT = 7.5f;

    private Transform pilotingContainer;
    private Transform pilotingCamera;
    private bool disableMovement;
    private bool disableTargeting;
    private bool isSteering;
    private Vector2 containerPos;
    private Vector2 containerVel;
    private float sqrDistFromCenter;
    private int raycastLayerMask;
    private FireWeaponAction weapon;
    private GameObject reticle;
    private Image reticleImage;

    private OVRPlayerController bodyObject;
    private Transform cameraObject;
    private Vector3 lastMousePos;
    private float sensitivity;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        bodyObject = body;
        cameraObject = camera;
        this.sensitivity = sensitivity;
        pilotingContainer = GameObject.Find(PILOTING_CONTAINER_NAME).transform;
        pilotingCamera = UnityUtils.FindGameObject(pilotingContainer.gameObject, PILOTING_CAMERA_NAME).transform;
        raycastLayerMask = LayerMask.GetMask(LAYER_SHOOTABLE);
        weapon = UnityUtils.FindGameObject(pilotingContainer.gameObject, WEAPON_NAME).GetComponent<FireWeaponAction>();
        reticle = UnityUtils.FindGameObject(GameObject.Find(BG_HUD_CANVAS),RETICLE_NAME);
        reticleImage = reticle.GetComponent<Image>();
        SetReticleEnabled(true);


        if (pilotingContainer != null)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
            Service.Controls.SetTriggerObserver(OnTriggerClicked);
            Service.UpdateManager.AddObserver(OnUpdate);
            Service.EventManager.AddListener(EventId.TogglePilotingControls, OnControlsToggled);
        }
        else
        {
            Debug.LogError("[PilotingControlScheme] Could not find piloting container object!");
        }
    }

    private void SetReticleEnabled(bool enabled)
    {
        if (reticle != null)
        {
            reticle.SetActive(enabled);
        }
        else
        {
            Debug.LogWarning("No reticle found!");
        }
    }

    private bool OnControlsToggled(object cookie)
    {
        TogglePilotingControlsAction toggleAction = (TogglePilotingControlsAction)cookie;
        SetMovementEnabled(toggleAction.UseSteering);
        disableTargeting = !toggleAction.UseTargeting;
        reticle.SetActive(toggleAction.UseTargeting);
        return true;
    }

    public void SetMovementEnabled(bool enabled)
    {
        disableMovement = !enabled;
    }

    public void Deactivate()
    {
        Service.Controls.RemoveTouchObserver(OnTouchUpdate);
        Service.Controls.RemoveTriggerObserver(OnTriggerClicked);
        Service.UpdateManager.RemoveObserver(OnUpdate);
        Service.EventManager.RemoveListener(EventId.TogglePilotingControls, OnControlsToggled);
        SetReticleEnabled(false);
    }

    private void OnTriggerClicked(TriggerUpdate update)
    {
        if (!disableTargeting && update.TriggerClicked)
        {
            weapon.Initiate();
        }
    }

    private void OnTouchUpdate(TouchpadUpdate update)
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

        bool isPressed = update.TouchpadPressState;
        if (false && !disableMovement && isPressed)
        {
            Vector2 touchDir = update.TouchpadPosition;
            touchDir.x *= -1f;
            float accel = touchDir.magnitude * ACCELERATION;

            bool isInBounds = sqrDistFromCenter < MAX_DIST_FROM_CENTER_SQR;

            bool applyX = isInBounds ||
                (touchDir.x < 0 && containerPos.x > 0) || 
                (touchDir.x > 0 && containerPos.x < 0);

            bool applyY = isInBounds ||
                (touchDir.y < 0 && containerPos.y > 0) ||
                (touchDir.y > 0 && containerPos.y < 0);

            isSteering = applyX || applyY;

            if (applyX)
            {
                containerVel.x += touchDir.x * accel;
            }

            if (applyY)
            {
                containerVel.y += touchDir.y * accel;
            }

            if (containerVel.sqrMagnitude > MAX_SPEED_SQR)
            {
                containerVel = containerVel.normalized * MAX_SPEED;
            }
        }
        else
        {
            isSteering = false;
        }
    }

    private void OnUpdate(float dt)
    {
        if (!isSteering)
        {
            containerVel *= (1f - DRAG * dt);
        }

        containerPos += containerVel * dt;
        sqrDistFromCenter = containerPos.sqrMagnitude;

        pilotingContainer.localPosition = containerPos;
        Vector3 euler = pilotingContainer.localEulerAngles;
        euler.z = -containerVel.x / MAX_SPEED * MAX_CAM_TILT;
        euler.x = containerVel.y / MAX_SPEED * MAX_CAM_TILT;
        pilotingContainer.localEulerAngles = euler;

        if (!disableTargeting)
        {
            Vector3 vectorToReticle = reticle.transform.position - pilotingCamera.position;
            Ray targetRay = new Ray(pilotingCamera.position, vectorToReticle);
            if (Physics.Raycast(targetRay, float.MaxValue, raycastLayerMask))
            {
                reticleImage.material.color = Color.red;
            }
            else
            {
                reticleImage.material.color = Color.white;
            }
        }
    }
}
