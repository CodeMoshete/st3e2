using UnityEngine;

public class MovementControlScheme : IControlScheme
{
    private bool disableMovement;
    private OVRPlayerController bodyObject;
    private Transform cameraObject;
    private bool hasPlayerTurned;
    private Vector3 lastMousePos;
    private float sensitivity;
    private bool isDebugMenuActive;

    public void Initialize(OVRPlayerController body, Transform camera, float sensitivity)
    {
        bodyObject = body;
        cameraObject = camera;
        this.sensitivity = sensitivity;

        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);
    }

    public void Deactivate()
    {
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
    }

    public void SetMovementEnabled(bool enabled)
    {
        disableMovement = !enabled;
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
        float dt = Time.deltaTime;

        bool isPressed = update.TouchpadPressState;
        bool isPressedThisFrame = update.TouchpadClicked;
        bool MoveUp = isPressed && !disableMovement && update.TouchpadPosition.y > 0.33;
        bool MoveDown = isPressed && !disableMovement && update.TouchpadPosition.y < -0.33;

        if (ControlsManager.Instance.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            OVRInput.Controller activeController = OVRInput.GetActiveController();
            Vector2 thumbPos = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, activeController);
            if (!hasPlayerTurned && Mathf.Abs(update.TouchpadPosition.x) > 0.7)
            {
                isPressedThisFrame = true;
                hasPlayerTurned = true;
            }
            else if (hasPlayerTurned && Mathf.Abs(update.TouchpadPosition.x) <= 0.7)
            {
                hasPlayerTurned = false;
            }
        }

        bool MoveLeft = isPressedThisFrame && update.TouchpadPosition.x < -0.33;
        bool MoveRight = isPressedThisFrame && update.TouchpadPosition.x > 0.33;

        //Log("V: " + primaryTouchpad.y + ", H: " + primaryTouchpad.x + ", Dn: " + isPressed);

        if (MoveUp)
        {
            bodyObject.transform.Translate(new Vector3(0f, 0f, 1f * dt));
        }
        else if (MoveDown)
        {
            bodyObject.transform.Translate(new Vector3(0f, 0f, -1f * dt));
        }

        Vector3 euler = bodyObject.transform.eulerAngles;
        if (!MoveUp && !MoveDown)
        {
            if (MoveLeft)
            {
                // euler.y -= 75f * dt;
                euler.y -= 45f;
            }
            else if (MoveRight)
            {
                // euler.y += 75f * dt;
                euler.y += 45f;
            }
            bodyObject.transform.eulerAngles = euler;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            euler = bodyObject.transform.eulerAngles;
            euler.y += dt * -((mouseDelta.x / Screen.width) * sensitivity);
            bodyObject.transform.eulerAngles = euler;

            euler = cameraObject.eulerAngles;
            euler.x += dt * (mouseDelta.y / Screen.height) * sensitivity;
            cameraObject.eulerAngles = euler;
        }
#endif
    }
}
