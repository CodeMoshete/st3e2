using UnityEngine;

public class InterpolateObjectByNameAction : ObjectReferenceActionBase
{
    public float Delay;
    public string TargetObjectName;
    private GameObject targetObject;
    public string TargetPositionName;
	private Transform targetPosition;
    public bool EaseIn;
    public bool EaseOut;
	public float MovementDuration;
    public CustomAction OnStart;
    public CustomAction OnComplete;
    private float currentDuration;
    private Vector3 playerStartPos;

	private bool isActive;

    public override void Initiate()
	{
        TryGetObjects();

        isActive = true;
        currentDuration = MovementDuration;

        if (targetObject != null)
        {
            playerStartPos = targetObject.transform.position;
        }

        if (OnStart != null)
        {
            OnStart.Initiate();
        }
	}
	
    private void TryGetObjects()
    {
        if (levelContent == null)
        {
            return;
        }

        if (targetObject == null)
        {
            targetObject = levelContent.GetInteractiveObject(TargetObjectName);
        }

        if (targetPosition == null)
        {
            GameObject targetPositionObj = levelContent.GetInteractiveObject(TargetPositionName);
            if (targetPositionObj != null)
            {
                targetPosition = targetPositionObj.transform;
            }
        }
    }

	void Update ()
    {
		if(isActive)
		{
            float dt = Time.deltaTime;
            if (Delay > 0f)
            {
                Delay -= dt;
                return;
            }

            currentDuration -= dt;

            if (targetObject != null && targetPosition != null)
            {
                float pct = 1 - currentDuration / MovementDuration;
                ApplyEase(ref pct);
                Vector3 nextPos = Vector3.Lerp(playerStartPos, targetPosition.transform.position, pct);
                targetObject.transform.position = nextPos;
            }

            if (currentDuration <= 0f)
            {
                FinishMovement();
            }
		}
	}

    void ApplyEase(ref float pct)
    {
        if (EaseIn && EaseOut)
        {
            if (pct > 0.5f)
            {
                pct = -(2f * Mathf.Pow(pct - 1f, 2f)) + 1;
            }
            else
            {
                pct = 2f * Mathf.Pow(pct, 2f);
            }
        }
        else if (EaseIn)
        {
            pct = pct * pct;
        }
        else if (EaseOut)
        {
            pct = -(Mathf.Pow(pct - 1f, 2f)) + 1f;
        }
    }

    private void FinishMovement()
    {
        if (targetObject != null && targetPosition != null)
        {
            targetObject.transform.position = targetPosition.transform.position;
        }

        isActive = false;

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
