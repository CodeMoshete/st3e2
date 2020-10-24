using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
	private Transform cameras;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(cameras)
		{
			Quaternion a = cameras.rotation;
			cameras.Rotate(new Vector3(5,5,5));
			//a.x = a.x+10;
			//a.y = a.y+10;
			//a.z = a.z+10;
			//a.w = a.w+10;
			//cameras.rotation = a;
			//print(a);
		}
	}
	
	void OnTriggerEnter(Collider collisionObject) {
		print ("Collision Detected " + collisionObject.tag);
        if(collisionObject.tag == "Player")
		{
			cameras = collisionObject.transform;//.Find("OVRCameraController");
			//collisionObject.GetComponent<OVRPlayerController>().animation.Play("SpinAnim");
			//collisionObject.GetComponent<OVRPlayerController>().Acceleration = 0;
			//collisionObject.GetComponent<OVRPlayerController>().GravityModifier = 0;
			//collisionObject.transform.parent = this.transform;
			//this.animation.Play("CinematicFlying2");
			Instantiate(Resources.Load("ExplosionHD"), new Vector3(2.668633f,1.349092f,-28.90166f), Quaternion.identity);
		}
    }
}
