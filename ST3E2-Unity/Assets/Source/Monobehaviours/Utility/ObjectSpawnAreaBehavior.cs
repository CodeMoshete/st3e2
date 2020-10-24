using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnObjectEntry
{
    public GameObject Prefab;
    public int SpawnWeight;
}

[RequireComponent(typeof(SphereCollider))]
public class ObjectSpawnAreaBehavior : MonoBehaviour
{
    public int NumObjectsToSpawn;
    public List<SpawnObjectEntry> ObjectsToSpawn;

    void Start()
    {
        List<GameObject> weightedObjects = new List<GameObject>();
        for(int i = 0, count = ObjectsToSpawn.Count; i < count; ++i)
        {
            for(int j = 0; j < ObjectsToSpawn[i].SpawnWeight; ++j)
            {
                weightedObjects.Add(ObjectsToSpawn[i].Prefab);
            }
        }

        SphereCollider bounds = gameObject.GetComponent<SphereCollider>();
        bounds.isTrigger = true;

        for (int i = 0; i < NumObjectsToSpawn; ++i)
        {
            int rand = Random.Range(0, weightedObjects.Count);
            Vector3 randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            float randRange = Random.Range(0f, 1f);
            randomPos = (randomPos.normalized * bounds.radius * randRange) + transform.position;
            Instantiate(weightedObjects[rand], randomPos, Random.rotation);
        }
    }
}
