using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCrowdManager : MonoBehaviour
{
    public int NumEntities;
    public List<GameObject> EntityTypes;
    public Transform MainCamera;
    private List<Transform> entities;
    private List<Material> entityMats;
    private int updateIndex = 0;

    private void Start()
    {
        entities = new List<Transform>();
        entityMats = new List<Material>();
        Vector3 pos = transform.position;
        BoxCollider spawnBounds = GetComponent<BoxCollider>();
        for (int i = 0; i < NumEntities; ++i)
        {
            float width = spawnBounds.bounds.size.x;
            float height = spawnBounds.bounds.size.y;
            float depth = spawnBounds.bounds.size.z;
            Vector3 spawnPosition = new Vector3(
                Random.Range(pos.x - width / 2, pos.x + width / 2),
                Random.Range(pos.y - height / 2, pos.y + height / 2),
                Random.Range(pos.z - depth / 2, pos.z + depth / 2));
            GameObject newEntity = Instantiate(EntityTypes[Random.Range(0, EntityTypes.Count)], transform);
            newEntity.transform.position = spawnPosition;
            entities.Add(newEntity.transform);
            entityMats.Add(newEntity.GetComponent<Renderer>().material);
        }
    }

    private int getHIndex(float componentValue)
    {
        int returnIndex = 0;
        if (componentValue < 0.75)
        {
            returnIndex++;
        }

        if (componentValue < 0.25)
        {
            returnIndex++;
        }

        if (componentValue < -0.25)
        {
            returnIndex++;
        }

        if (componentValue < -0.75)
        {
            returnIndex++;
        }
        return returnIndex;
    }

    private int getVIndex(float componentValue)
    {
        int returnIndex = 1;
        if (componentValue < 0.5)
        {
            returnIndex++;
        }

        if (componentValue < -0.5)
        {
            returnIndex++;
        }
        return returnIndex;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        //for (int i = 0, count = entities.Count; i < count; ++i)
        for (int i = 0; i < 100; ++i)
        {
            Transform entity = entities[updateIndex];
            entity.Translate(new Vector3(0f, 0f, 1f * dt));
            Vector3 vecToEntity = Vector3.Normalize(MainCamera.position - entity.position);
            float zComponent = Vector3.Dot(entity.forward, vecToEntity);
            float xComponent = Vector3.Dot(entity.right, vecToEntity);
            float yComponent = Vector3.Dot(entity.up, vecToEntity);

            int verticalIndex = getVIndex(yComponent);
            int horizontalIndex = getHIndex(zComponent);
            int flip = xComponent < 0 ? -1 : 1;

            //Debug.Log("H: " + horizontalIndex + ", V: " + verticalIndex + ", F: " + flip);
            Material mat = entityMats[updateIndex];
            mat.SetInt("_HIndex", horizontalIndex);
            mat.SetInt("_VIndex", verticalIndex);
            mat.SetInt("_Flip", flip);

            updateIndex += 1;
            if (updateIndex >= NumEntities)
            {
                updateIndex = 0;
            }
        }
    }
}
