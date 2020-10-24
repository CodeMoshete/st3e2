using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FireAction
{
    public float Delay;
    public Transform TargetPosition;
    public Collider TargetCollider;
    public CustomAction OnHit;
}

public class FireWeaponAction : FireWeaponBaseAction
{
    public GameObject ProjectilePrefab;
    public float WeaponVelocity;
    public Transform SourceParent;
    public CustomAction OnStart;
    public List<FireAction> FireActions;

    private WeaponBase weapon;

    public override void Initiate()
    {
        for (int i = 0, count = FireActions.Count; i < count; ++i)
        {
            Service.TimerManager.CreateTimer(FireActions[i].Delay, ExecuteFireAction, FireActions[i]);
        }

        if (OnStart != null)
        {
            OnStart.Initiate();
        }
    }

    private void ExecuteFireAction(object cookie)
    {
        GameObject projectile = Instantiate(ProjectilePrefab);
        weapon = projectile.GetComponent<WeaponBase>();
        FireAction action = (FireAction)cookie;
        weapon.Fire(action, SourceParent, WeaponVelocity);
    }

    public override void CeaseFire()
    {
        if (weapon != null)
        {
            weapon.CeaseFire();
        }
    }
}
