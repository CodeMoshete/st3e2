using UnityEngine;

public class PhaserWeapon : WeaponBase
{
    private const float DESTROY_TIME = 0.75f;
    private const float DEFAULT_SCALE_SPEED = 5f;

    public Transform EndPoint;
    private float lifespan = 5f;
    private Vector3 scaleSpeed;
    private Vector3 startScale;
    private bool shouldDestroy = false;
    private float destroyTime;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);

        float zSpeed = Velocity > 0f ? Velocity : DEFAULT_SCALE_SPEED;
        scaleSpeed = new Vector3(0f, 0f, zSpeed);
        transform.SetParent(SourceParent);
        transform.localPosition = Vector3.zero;
        destroyTime = DESTROY_TIME;
        startScale = transform.localScale;
        transform.localScale = new Vector3(1f, 1f, 0.001f);
    }

    private void Update()
    {
        transform.LookAt(InitialPosition);

        if (shouldDestroy)
        {
            if (destroyTime > 0f)
            {
                destroyTime -= Time.deltaTime;
                float dt = destroyTime / DESTROY_TIME;
                Vector3 localScale = transform.localScale;
                localScale.x = startScale.x * dt;
                localScale.y = startScale.y * dt;
                transform.localScale = localScale;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.localScale += scaleSpeed;

            lifespan -= Time.deltaTime;
            shouldDestroy = lifespan <= 0f;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (TargetCollider == null || other == TargetCollider)
        {
            if (OnHitCustomAction != null)
            {
                OnHitCustomAction.Initiate();
            }

            Vector3 hitFxPoint = EndPoint.position;
            Ray ray = new Ray(transform.position, EndPoint.position - transform.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            for (int i = 0, count = hits.Length; i < count; ++i)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == TargetCollider)
                {
                    hitFxPoint = hit.point;
                }
            }

            float distToPoint = Vector3.Distance(hitFxPoint, transform.position);
            Transform cachedParent = transform.parent;
            transform.SetParent(null);
            Vector3 scale = transform.localScale;
            scale.z = distToPoint;
            transform.localScale = scale;
            transform.SetParent(cachedParent);

            GameObject explosion = Instantiate(HitFX, hitFxPoint, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            shouldDestroy = true;
        }
    }
}
