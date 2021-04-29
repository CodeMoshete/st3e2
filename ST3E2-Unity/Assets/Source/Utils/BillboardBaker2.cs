using UnityEngine;

public class BillboardBaker2 : MonoBehaviour
{
    private const float TIMER_TIME = 0.5f;

    public Camera ScreenshotCamera;
    public SR_RenderCamera RenderCamera;
    public GameObject TargetObject;
    public float Distance;

    private Transform cameraTransform;
    private float horizontalAngle = 0f;
    private float verticalAngle = -90f;
    private float timer = TIMER_TIME;
    private int rotationCounter = 0;
    private int totalNumRotations = 0;

    private void Start()
    {
        cameraTransform = ScreenshotCamera.transform;
    }

    private void Update()
    {
        if (totalNumRotations >= 25)
        {
            Debug.Log("Done!");
            return;
        }

        if (timer <= 0f)
        {
            RenderCamera.CamCapture();
            totalNumRotations++;
            horizontalAngle += 45f;
            timer = TIMER_TIME;
            rotationCounter++;
            if (rotationCounter >= 5)
            {
                horizontalAngle = 0f;
                rotationCounter = 0;
                verticalAngle += 45f;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }

        float yPos = Mathf.Sin(verticalAngle * Mathf.Deg2Rad) * Distance;
        float hRadius = Mathf.Cos(verticalAngle * Mathf.Deg2Rad) * Distance;
        float xPos = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * hRadius;
        float zPos = Mathf.Cos(horizontalAngle * Mathf.Deg2Rad) * hRadius;
        cameraTransform.localPosition = new Vector3(xPos, yPos, zPos);
        cameraTransform.LookAt(TargetObject.transform);

        int diff = Mathf.RoundToInt(Mathf.Abs(Mathf.Abs(verticalAngle - 90f)));
        //Debug.Log("Angle: " + diff + ", " + (diff == 0));
        if (diff == 0 || diff == 180)
        {
            bool bottom = diff == 180;
            Vector3 currentAngles = cameraTransform.localEulerAngles;
            currentAngles.z = bottom ? 180f + horizontalAngle : 180 - horizontalAngle;
            cameraTransform.localEulerAngles = currentAngles;
        }
    }
}
