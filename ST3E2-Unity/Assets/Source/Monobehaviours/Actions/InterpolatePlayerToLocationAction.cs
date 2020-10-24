using UnityEngine;

public class InterpolatePlayerToLocationAction : CustomAction
{
    public float Delay;
    public GameObject Player;
	public Transform TargetPosition;
    public bool EaseIn;
    public bool EaseOut;
	public float MovementDuration;
    private float currentDuration;
    private Vector3 playerStartPos;

	private bool isActive;

	public override void Initiate()
	{
		isActive = true;
        currentDuration = MovementDuration;
        playerStartPos = Player.transform.position;
        Player.GetComponent<CharacterController>().enabled = false;
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
            float pct = 1 - currentDuration / MovementDuration;
            ApplyEase(ref pct);
            Vector3 nextPos = Vector3.Lerp(playerStartPos, TargetPosition.transform.position, pct);
            Player.transform.position = nextPos;

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
                pct = -(2f * Mathf.Pow(pct - 1f, 2f) + 1);
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
        Player.transform.position = TargetPosition.transform.position;
        isActive = false;
        Player.GetComponent<CharacterController>().enabled = true;
    }
}
