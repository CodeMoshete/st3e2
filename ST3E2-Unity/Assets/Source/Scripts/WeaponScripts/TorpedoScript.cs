using UnityEngine;
using System.Collections;

public class TorpedoScript : MonoBehaviour {
	private const float SPIN_RATE = 2.0f;
	public float TORPEDO_SPEED = 5.0f;
	public float LIFESPAN_SEC = 8.0f;

	private GameObject player;
	private Transform innerCont;
	private Transform torpFront;
	private Transform torpBack;

	private bool shouldDestroy = false;
	private float scheduledDestroy;

	private Vector3 zRotationBack = new Vector3(0.0f, 0.0f, SPIN_RATE);
	private Vector3 zRotationFront = new Vector3(0.0f, 0.0f, -SPIN_RATE);

	private Vector3 velocity = Vector3.zero;
	public GameObject Launcher;

	// Use this for initialization
	void Start () {
		innerCont = transform.Find("InnerContainer");
		torpBack = innerCont.Find("Back");
		torpFront = innerCont.Find("Front");
		player = GameObject.Find("OVRPlayerController");
		scheduledDestroy = Time.fixedTime + LIFESPAN_SEC;
	}
	
	// Update is called once per frame
	void Update () {
		innerCont.LookAt(player.transform.position);
		this.transform.Translate(velocity * Time.deltaTime);
		torpBack.Rotate(zRotationBack);
		torpFront.Rotate(zRotationFront);

		if(shouldDestroy)
			Destroy(this.gameObject);
	}

	void FixedUpdate()
	{
		if(Time.fixedTime >= scheduledDestroy)
		{
			shouldDestroy = true;
		}
	}

	public void setVelocity(Vector3 vel)
	{
		velocity = vel;
	}
}
