using UnityEngine;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;

public class FlockController : MonoBehaviour
{
    private struct FlockerRef
    {
        public GameObject GameObject;
        public Transform Transform;
        public ShipFlocker FlockerComp;

        public FlockerRef(GameObject original)
        {
            GameObject = original;
            Transform = original.transform;
            FlockerComp = original.GetComponent<ShipFlocker>();
        }
    }

	private const float MIN_COHESION_DIST = 64.0f;
	private const float MAX_COHESION_DIST = 300.0f;
	private const float AVOIDANCE_DIST = 1000f;
	private const float AIM_VARIATION = 1.0f;
	public int numToSpawn = 20;
	public int spawnAreaDims = 5;
	public float FlockScale = 1.0f;
	public List<string> possibleSpawns;
	public Vector3 spawnOrientation = new Vector3(0.0f, 0.0f, 0.0f);
	private int numInFlock;
	private List<FlockerRef> fullFlock;
	private Vector3 flockCenter;
	private GameObject flockTarget;
	private FlockerRef attackTarget;

    private bool isUsingJobs = true;

	// Use this for initialization
	void Start () {
		attackTarget = new FlockerRef(GameObject.Find("BorgCube"));

		possibleSpawns = new List<string>();
		possibleSpawns.Add("EnterpriseShipContainer");
		possibleSpawns.Add("SovereignShipContainer");
		possibleSpawns.Add("AkiraShipContainer");
		possibleSpawns.Add("SteamrunnerShipContainer");
		possibleSpawns.Add("DefiantShipContainer");
		possibleSpawns.Add("SaberShipContainer");
		possibleSpawns.Add("AmbassadorShipContainer");
		possibleSpawns.Add("NebulaShipContainer");

		int numPossibleSpawns = possibleSpawns.Count;

		fullFlock = new List<FlockerRef>();
		for(int i=0; i<numToSpawn; i++) 
		{
			Vector3 spawnPosition = 
                new Vector3(transform.position.x + Random.Range(-spawnAreaDims,spawnAreaDims),
			                transform.position.y + Random.Range(-spawnAreaDims,spawnAreaDims), 
			                transform.position.z + Random.Range(-spawnAreaDims,spawnAreaDims));
			//Vector3 spawnPosition = new Vector3(transform.position.x - 5.0f, transform.position.y +5.0f, transform.position.z + 5.0f);
			//GameObject tmpObject = (GameObject) Instantiate(Resources.Load("EnterpriseShipContainer"), spawnPosition, Quaternion.Euler(spawnOrientation));
			string chosenSpawner = possibleSpawns[Random.Range(0,numPossibleSpawns)];

			GameObject tmpObject = Instantiate(
                Resources.Load<GameObject>(chosenSpawner), 
                spawnPosition, 
                Quaternion.Euler(spawnOrientation));

			//tmpObject.transform.parent = this.transform;
			tmpObject.transform.localScale = new Vector3(FlockScale, FlockScale, FlockScale);
			fullFlock.Add(new FlockerRef(tmpObject));
		}
		flockTarget = GameObject.Find("FlockTarget");
		numInFlock = numToSpawn;

        Service.EventManager.AddListener(EventId.DebugToggleJobs, ToggleJobs);
	}
	
    private bool ToggleJobs(object cookie)
    {
        isUsingJobs = (bool)cookie;
        return true;
    }

	// Update is called once per frame
	void Update () {
		updateFlockCenter();
		for(int i=0; i<numInFlock; i++)
		{
			processFlockerMovement(fullFlock[i]);
			processFlockerAttack(fullFlock[i]);
		}

		processFlockerAttack(attackTarget);
		attackTarget.Transform.Rotate(new Vector3(0.0f,Time.deltaTime,0.0f));
	}

	void updateFlockCenter()
	{
        NativeArray<Vector3> positions = new NativeArray<Vector3>(numInFlock, Allocator.TempJob);
        NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);

        for (int i = 0; i < numInFlock; i++)
        {
            positions[i] = fullFlock[i].Transform.position;
        }

        FindCenterJob jobData = new FindCenterJob();
        jobData.FlockerPositions = positions;
        jobData.Result = result;
        JobHandle handle = jobData.Schedule();
        handle.Complete();
        flockCenter = result[0];
        positions.Dispose();
        result.Dispose();
	}

	void processFlockerMovement(FlockerRef flocker)
	{
		Vector3 avoidanceTest = getAvoidanceVector(flocker);
		Vector3 desiredVelocity = flocker.Transform.forward;
		if(avoidanceTest != Vector3.zero)
		{
			//print ("Avoiding collision!");
			desiredVelocity = avoidanceTest;
		}
		else
		{
			updateFlockerBehaviorWeights(flocker);

			float cohesionWeight = flocker.FlockerComp.cohesionWeight;
			float dispersionWeight = flocker.FlockerComp.dispersionWeight;
			float seekWeight = flocker.FlockerComp.seekWeight;

			float selection = Random.Range(0.0f, (cohesionWeight+dispersionWeight+seekWeight));
			//print("selection: " + selection + ", weight: " + cohesionWeight+dispersionWeight+seekWeight);

			if(selection < cohesionWeight)
			{
				//print ("cohesion");
				Vector3 cohesionComponent = applyCohesion(flocker);
				desiredVelocity = cohesionComponent;
				//cohesionComponent.Scale(new Vector3(cohesionScale,cohesionScale,cohesionScale));
			}
			else if(selection > cohesionWeight && selection < (cohesionWeight + dispersionWeight))
			{
				//print ("dispersion");
				Vector3 dispersionComponent = applyDispersion(flocker);
				desiredVelocity = dispersionComponent;
				//cohesionComponent.Scale(new Vector3(dispersionScale,dispersionScale,dispersionScale));
			}
			else if(selection > (cohesionWeight + dispersionWeight))
			{
				//print ("seek");
				if(flockTarget == null)
					flockTarget = this.gameObject;

				Vector3 seekRotation = applySeek(flocker, flockTarget.transform.position);
				desiredVelocity = seekRotation;
			}

			//desiredVelocity += seekComponent += cohesionComponent += dispersionComponent;
			//desiredVelocity = dispersionComponent;
		}

		flocker.Transform.Rotate(desiredVelocity.x, desiredVelocity.y, desiredVelocity.z);
		flocker.Transform.Translate(
            new Vector3(0.0f, 0.0f, flocker.FlockerComp.MaxSpeed * Time.deltaTime));
	}

	void processFlockerAttack(FlockerRef flocker)
	{
		bool isBadGuy = flocker.Transform == attackTarget.Transform;
		FlockerRef target = (isBadGuy) ? fullFlock[Random.Range(0,numToSpawn-1)] : attackTarget;
		ShipFlocker flockComp = flocker.FlockerComp;
		bool shootPhasers = 
            ((flockComp.PhaserShotsPerMinute/60.0f) * (Time.deltaTime)) > Random.Range(0.0f, 1.0f);
		if(shootPhasers)
		{
			//print ("Firing phasers!");
			GameObject closestPhaser = flockComp.phaserPoints[0];
			float closestDist = Mathf.Infinity;
			foreach(GameObject phsPt in flockComp.phaserPoints)
			{
				Vector3 distVec = phsPt.transform.position - target.Transform.position;
				float sqrDst = 
                    Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);
				if(sqrDst < closestDist)
				{
					closestDist = sqrDst;
					closestPhaser = phsPt;
				}
			}
			string phaserToLoad = isBadGuy ? "BorgPhaser" : "FederationPhaser";
			GameObject phaserObj = 
                Instantiate(Resources.Load<GameObject>(phaserToLoad),
                closestPhaser.transform.position,Quaternion.identity);

			phaserObj.transform.parent = flocker.Transform;
			phaserObj.GetComponent<PhaserScript>().Launcher = flocker.GameObject;
			phaserObj.GetComponent<PhaserScript>().setTarget(target.Transform);
		}

		bool shootTorpedoes = ((flockComp.TorpedoShotsPerMinute/60.0f) * (Time.deltaTime)) > Random.Range(0.0f, 1.0f);
		if(shootTorpedoes)
		{
			GameObject closestTorpedo = flockComp.torpedoPoints[0];
			float closestDist = Mathf.Infinity;
			foreach(GameObject trpPt in flockComp.torpedoPoints)
			{
				Vector3 distVec = trpPt.transform.position - target.Transform.position;
				float sqrDst = 
                    Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);

				if(sqrDst < closestDist)
				{
					closestDist = sqrDst;
					closestTorpedo = trpPt;
				}
			}
			string torpedoToLoad = isBadGuy ? "BorgTorpedo" : flockComp.TorpedoType;

			GameObject newTorp = 
                Instantiate(Resources.Load<GameObject>(torpedoToLoad),
                closestTorpedo.transform.position,Quaternion.identity);

			TorpedoScript torp = newTorp.GetComponent<TorpedoScript>();
			Vector3 aimPos = new Vector3(
                target.Transform.position.x + Random.Range(-AIM_VARIATION,AIM_VARIATION),
			    target.Transform.position.y + Random.Range(-AIM_VARIATION,AIM_VARIATION),
			    target.Transform.position.z + Random.Range(-AIM_VARIATION,AIM_VARIATION));
			Vector3 torpVel = (aimPos - closestTorpedo.transform.position).normalized;
			torpVel.Scale(new Vector3(torp.TORPEDO_SPEED,torp.TORPEDO_SPEED,torp.TORPEDO_SPEED));
			torp.setVelocity(torpVel);
			torp.Launcher = flocker.GameObject;
		}
	}

	Vector3 applyCohesion(FlockerRef flocker)
	{	
		return applySeek(flocker, flockCenter);
	}

	Vector3 applyDispersion(FlockerRef flocker)
	{
		return applyFlee(flocker, flockCenter);
	}
	
	Vector3 applySeek(FlockerRef flocker, Vector3 target)
	{
        if (isUsingJobs)
        {
            NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);
            FlockerSeekJob jobData = new FlockerSeekJob();
            jobData.FlockerPosition = flocker.Transform.position;
            jobData.EulerAngles = flocker.Transform.localEulerAngles;
            jobData.TargetPosition = target;
            jobData.MaxPitch = flocker.FlockerComp.MaxPitch;
            jobData.TurnSpeed = flocker.FlockerComp.TurnSpeed;
            jobData.Result = result;
            JobHandle handle = jobData.Schedule();
            handle.Complete();
            Vector3 retVec = result[0];
            result.Dispose();
            return retVec;
        }
        else
        {
            ShipFlocker flockComp = flocker.FlockerComp;
            float turnSpd = flockComp.TurnSpeed;
            Vector3 directionVector = flocker.Transform.position - target;
            Vector3 orientation = flockComp.localEulerAngles;

            float yawDegrees =
                Mathf.DeltaAngle(orientation.y,
                (Mathf.Atan2(directionVector.x, directionVector.z) / (Mathf.PI / 180))) + 180;

            float desiredYawRotation = flockComp.localEulerAngles.y;
            if (yawDegrees <= 180)
                desiredYawRotation = Mathf.Min(yawDegrees, turnSpd);
            else if (yawDegrees > 180)
                desiredYawRotation = Mathf.Max(-yawDegrees, -turnSpd);

            float desiredPitchRotation = 0;

            if (flocker.Transform.position.y > target.y &&
                ((flockComp.localEulerAngles.x > 180) ||
                (flockComp.localEulerAngles.x < flockComp.MaxPitch)))
            {
                desiredPitchRotation = turnSpd / 3;
            }
            else if (flocker.Transform.position.y < target.y &&
                (flockComp.localEulerAngles.x < 180) ||
                (flockComp.localEulerAngles.x > (360.0f - flockComp.MaxPitch)))
            {
                desiredPitchRotation = -turnSpd / 3;
            }

            Vector3 retVec = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);

            return retVec;
        }
	}

	Vector3 applyFlee(FlockerRef flocker, Vector3 target)
	{
        if (isUsingJobs)
        {
            NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);
            FlockerFleeJob jobData = new FlockerFleeJob();
            jobData.FlockerPosition = flocker.Transform.position;
            jobData.EulerAngles = flocker.Transform.localEulerAngles;
            jobData.TargetPosition = target;
            jobData.MaxPitch = flocker.FlockerComp.MaxPitch;
            jobData.TurnSpeed = flocker.FlockerComp.TurnSpeed;
            jobData.Result = result;
            JobHandle handle = jobData.Schedule();
            handle.Complete();
            Vector3 retVec = result[0];
            result.Dispose();
            return retVec;
        }
        else
        {
            ShipFlocker flockComp = flocker.FlockerComp;
            float turnSpd = flockComp.TurnSpeed;
            Vector3 directionVector = flocker.Transform.position - target;
            Vector3 orientation = flockComp.localEulerAngles;

            float yawDegrees =
                Mathf.DeltaAngle(orientation.y,
                (Mathf.Atan2(directionVector.x, directionVector.z) / (Mathf.PI / 180))) + 180;

            float desiredYawRotation = flockComp.localEulerAngles.y;
            if (yawDegrees >= 180)
                desiredYawRotation = Mathf.Min(yawDegrees, turnSpd);
            else if (yawDegrees < 180)
                desiredYawRotation = Mathf.Max(-yawDegrees, -turnSpd);

            float desiredPitchRotation = 0;

            if (flocker.Transform.position.y < target.y &&
                ((flockComp.localEulerAngles.x > 180) ||
                (flockComp.localEulerAngles.x < flockComp.MaxPitch)))
            {
                desiredPitchRotation = turnSpd / 3;
            }
            else if (flocker.Transform.position.y > target.y &&
                (flockComp.localEulerAngles.x < 180) ||
                (flockComp.localEulerAngles.x > (360.0f - flockComp.MaxPitch)))
            {
                desiredPitchRotation = -turnSpd / 3;
            }

            Vector3 retVec = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);

            return retVec;
        }
    }

    Vector3 getAvoidanceVector(FlockerRef flocker)
	{
        if (isUsingJobs)
        {
            NativeArray<Vector3> result = new NativeArray<Vector3>(1, Allocator.TempJob);
            FlockerAvoidanceJob jobData = new FlockerAvoidanceJob();
            jobData.FlockerPosition = flocker.Transform.position;
            jobData.EulerAngles = flocker.Transform.localEulerAngles;
            jobData.TargetPosition = attackTarget.Transform.position;
            jobData.Result = result;
            jobData.MaxPitch = flocker.FlockerComp.MaxPitch;
            jobData.TurnSpeed = flocker.FlockerComp.TurnSpeed;
            jobData.AvoidanceDistance = AVOIDANCE_DIST;
            JobHandle handle = jobData.Schedule();
            handle.Complete();
            Vector3 retVec = result[0];
            result.Dispose();
            return retVec;
        }
        else
        {
            if (attackTarget.Transform != null)
            {
                Vector3 fpos = flocker.Transform.position;
                Vector3 bpos = attackTarget.Transform.position;
                Vector3 distVec = fpos - bpos;
                float sqrDist =
                    (Mathf.Pow(distVec.x, 2) + Mathf.Pow(distVec.y, 2) + Mathf.Pow(distVec.z, 2));

                if (sqrDist <= AVOIDANCE_DIST)
                    return applyFlee(flocker, attackTarget.Transform.position);
            }

            return Vector3.zero;
        }
    }

	private void updateFlockerBehaviorWeights(FlockerRef flocker)
	{
		Vector3 distVec = flocker.Transform.position - flockCenter;
		float sqrDist = Mathf.Pow(distVec.x,2) + Mathf.Pow(distVec.y,2) + Mathf.Pow(distVec.z,2);

		//print(sqrDist);
		float cohesionWeight = 
            Mathf.Min(Mathf.Max(sqrDist - MIN_COHESION_DIST, 0.0f) / 
            (MAX_COHESION_DIST - MIN_COHESION_DIST), 1.0f);
		//print("cohesion: " + cohesionWeight + ", dispersion: " + (20-cohesionWeight));
		flocker.FlockerComp.cohesionWeight = cohesionWeight * 3;
		flocker.FlockerComp.dispersionWeight = 3-cohesionWeight;
	}
}