using UnityEngine;

public class TorpedoWeapon : WeaponBase
{
    public bool OrientTowardsTarget;
    private Vector3 movementDir;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);
        transform.position = SourceParent.position;
        movementDir = Vector3.Normalize(InitialPosition - transform.position) * Velocity;

        if (OrientTowardsTarget)
        {
            transform.LookAt(transform.position + movementDir);
        }
    }

    private void Update()
    {
        if (OrientTowardsTarget)
        {
            transform.Translate(Vector3.forward * Velocity * Time.deltaTime);
        }
        else
        {
            transform.Translate(movementDir * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (TargetCollider == null || other == TargetCollider)
        {
            if (OnHit != null)
            {
                OnHit.Invoke();
            }

            if (OnHitCustomAction != null)
            {
                OnHitCustomAction.Initiate();
            }

            GameObject explosion = Instantiate(HitFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
