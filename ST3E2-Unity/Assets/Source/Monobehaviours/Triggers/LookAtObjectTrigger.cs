using UnityEngine;

public class LookAtObjectTrigger : MonoBehaviour
{
    public Camera Camera;
    public CustomAction OnLookAt;
    public CustomAction OnLookAway;
    public CustomAction OnInteract;

    private bool isColliding;
    private int testLayer;

    public void Start()
    {
        testLayer = LayerMask.GetMask("Interactible");
    }

    public void OnDisable()
    {
        if (isColliding)
        {
            isColliding = false;
            if (OnInteract != null)
            {
                Service.Controls.RemoveTriggerObserver(OnTriggerUPdate);
            }
        }
    }

    public void Update ()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
        if (Physics.Raycast(ray, out hit, 5f, testLayer) && hit.collider.gameObject == gameObject)
        {
            if (!isColliding)
            {
                if (OnLookAt != null)
                {
                    OnLookAt.Initiate();
                }
                isColliding = true;
                if (OnInteract != null)
                {
                    Service.Controls.SetTriggerObserver(OnTriggerUPdate);
                }
                Debug.Log("LookAt");
            }
        }
        else if (isColliding)
        {
            if (OnLookAway != null)
            {
                OnLookAway.Initiate();
            }

            if (OnInteract != null)
            {
                Service.Controls.RemoveTriggerObserver(OnTriggerUPdate);
            }

            isColliding = false;
        }
    }

    private void OnTriggerUPdate(TriggerUpdate update)
    {
        if (update.TriggerClicked)
        {
            OnInteract.Initiate();
        }
    }
}
