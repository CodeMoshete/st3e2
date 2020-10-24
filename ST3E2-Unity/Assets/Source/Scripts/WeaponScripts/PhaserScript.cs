using UnityEngine;

public class PhaserScript : MonoBehaviour {
	public float SCALE_SPEED = 5.0f;
	public float LIFESPAN_SEC = 2f;

	private Transform phaser;
	
	private bool shouldDestroy = false;
	private float scheduledDestroy;
	private Transform target;
	private Vector3 scaleSpd;
	public Vector3 destroyScaleSpd = new Vector3(0.001f, 0.001f, 0.0f);

	public GameObject Launcher;

	void Start () {

		phaser = this.transform;
		scheduledDestroy = Time.fixedTime + LIFESPAN_SEC;
		scaleSpd = new Vector3(0.0f, 0.0f, SCALE_SPEED);
        if (target != null)
        {
            phaser.LookAt(target);
        }
        phaser.localScale = new Vector3(1f, 1f, 0f);
    }
	
	void Update () {
		if(target != null)
		{
			phaser.LookAt(target);
		}
		
		if(shouldDestroy)
		{
			//print ("destroying: " + phaser.localScale.x);
			if(phaser.localScale.x > 0)
				phaser.localScale -= destroyScaleSpd;
			else
				Destroy(this.gameObject);
		}
		else
			phaser.localScale += scaleSpd;
	}

	void OnTriggerEnter(Collider collisionObject) {
		if(collisionObject.tag != "Torpedo" && 
            collisionObject.tag != "Phaser" && 
            Launcher != collisionObject.gameObject)
		{
			Vector3 endPos = transform.Find("EndPt").transform.position;

			GameObject explosion = 
                Instantiate<GameObject>(
                    Resources.Load<GameObject>("ExplosionLD"), endPos, Quaternion.identity);

			explosion.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
			shouldDestroy = true;
		}
	}

	void FixedUpdate()
	{
		if(Time.fixedTime >= scheduledDestroy)
		{
			shouldDestroy = true;
		}
	}

	public void setTarget(Transform pt)
	{
		target = pt;
	}
}
