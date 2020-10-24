using UnityEngine;
using System.Collections;

public class ExplosionAreaBehavior : MonoBehaviour {
	public float ExplosionsPerMinute = 30.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bool doSpawn = ((ExplosionsPerMinute/60.0f) * (Time.deltaTime)) > Random.Range(0.0f, 1.0f);
		if(doSpawn)
		{
			Transform t = this.transform;
			BoxCollider b = GetComponent<BoxCollider>();
			float width = b.bounds.size.x;
			float height = b.bounds.size.y;
			float depth = b.bounds.size.z;
			Vector3 spawnPosition = new Vector3(Random.Range(t.position.x-width/2,t.position.x + width/2),
			                                    Random.Range(t.position.y-height/2,t.position.y + height/2),
			                                    Random.Range(t.position.z-depth/2,t.position.z + depth/2));
			Instantiate(Resources.Load("ExplosionLD"),spawnPosition,Random.rotation);
		}
	}
}
