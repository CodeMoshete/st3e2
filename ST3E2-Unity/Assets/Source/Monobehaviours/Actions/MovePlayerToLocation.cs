using UnityEngine;

public class MovePlayerToLocation : CustomAction {

    public float Delay;
    public GameObject Player;
	public GameObject TargetPosition;
	public float InitialVelocity;
    public MovePlayerToLocation InheritedVelocity;
    public bool disablePrevious;
    public bool IsInstantaneous;
	public float Acceleration;

	private bool isActive;

	public override void Initiate()
	{
		isActive = true;
		Player.GetComponent<CharacterController>().enabled = false;
        if (InheritedVelocity != null)
        {
            InitialVelocity = InheritedVelocity.InitialVelocity;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isActive)
		{
            if (IsInstantaneous)
            {
                FinishMovement();
                return;
            }

            if (Delay > 0f)
            {
                Delay -= Time.deltaTime;
                return;
            }

			InitialVelocity += Acceleration * Time.deltaTime;
			Vector3 vectorToTarget = TargetPosition.transform.position - Player.transform.position;

			if(vectorToTarget.sqrMagnitude < InitialVelocity * InitialVelocity)
			{
                FinishMovement();
            }
			else
			{
				Vector3 nextPos = (vectorToTarget).normalized * InitialVelocity;
				Vector3 currentPos = Player.transform.position;
				Player.transform.position = currentPos + nextPos;
			}
		}
	}

    private void FinishMovement()
    {
        Player.transform.position = TargetPosition.transform.position;
        isActive = false;
        Player.GetComponent<CharacterController>().enabled = true;
    }
}
