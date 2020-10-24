using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipFlocker : MonoBehaviour
{
	public float TurnSpeed = 0.01f;
	public float MaxPitch = 70.0f;
	public float Acceleration = 0.1f;
	public float MaxSpeed = 0.01f;

	public float cohesionWeight = 0.5f;
	public float dispersionWeight = 1.0f;
	public float seekWeight = 0.2f;

	public float PhaserShotsPerMinute = 5.0f;
	public float TorpedoShotsPerMinute = 5.0f;

	public string TeamLabel = "Team1";
	public string TorpedoType = "PhotonTorpedo";

	public Vector3 velocity = Vector3.zero;

	private Vector3 myEuler;

	// Use this for initialization
	void Start () {
		updateVelocityFromForward();
		myEuler = transform.localEulerAngles;
	}

	public void updateVelocityFromForward()
	{
		Vector3 fw = transform.forward.normalized;
		fw.Scale(new Vector3(MaxSpeed, MaxSpeed, MaxSpeed));
		velocity = fw;
		//print ("velocity now set to: " + velocity);
	}

	public void Rotate(float ax, float ay, float az)
	{
		myEuler += new Vector3(ax,ay,az); // rotate angles
		// assign the new angles in modulo 360:
		transform.localEulerAngles = new Vector3(myEuler.x%360,myEuler.y%360,myEuler.z%360);
	}

	public Vector3 localEulerAngles
	{
		get {
			if(myEuler.x > 360.0f)
				myEuler.x -= 360.0f;
			else if(myEuler.x < 0)
				myEuler.x += 360.0f;

			if(myEuler.y > 360.0f)
				myEuler.y -= 360.0f;
			else if(myEuler.y < 0)
				myEuler.y += 360.0f;

			if(myEuler.z > 360.0f)
				myEuler.z -= 360.0f;
			else if(myEuler.z < 0)
				myEuler.z += 360.0f;

			return myEuler;
		}
	}

	public GameObject[] phaserPoints
	{
		get
		{
			List<GameObject> retArr = new List<GameObject>();
			for(int i=0; i<transform.childCount; i++)
			{
				GameObject child = transform.GetChild(i).gameObject;
				if(child.name == "PhaserPoint")
					retArr.Add(child);
			}
			return retArr.ToArray();
		}
	}

	public GameObject[] torpedoPoints
	{
		get{
			List<GameObject> retArr = new List<GameObject>();
			for(int i=0; i<transform.childCount; i++)
			{
				GameObject child = transform.GetChild(i).gameObject;
				if(child.name == "TorpedoPoint")
					retArr.Add(child);
			}
			return retArr.ToArray();
		}
	}

	void OnTriggerEnter(Collider collisionObject) {
		if(collisionObject.tag == "Torpedo" && collisionObject.gameObject.GetComponent<TorpedoScript>().Launcher != this.gameObject)
		{
            Destroy(collisionObject.gameObject);
			GameObject explosion = (GameObject)Instantiate(Resources.Load("ExplosionLD"), collisionObject.transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
