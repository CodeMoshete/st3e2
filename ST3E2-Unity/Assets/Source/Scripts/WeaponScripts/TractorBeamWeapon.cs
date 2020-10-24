using UnityEngine;

public class TractorBeamWeapon : WeaponBase
{
    private const string ACTIVATE_TRIGGER = "Activate";
    private const string DEACTIVATE_TRIGGER = "Deactivate";
    public Animator Animator;
    public Transform Beam;
    public float OvershootPct = 1;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);
        Animator.SetTrigger(ACTIVATE_TRIGGER);
    }

    private void Update()
    {
        transform.position = SourceParent.position;
        transform.LookAt(Target.position);
        Vector3 newScale = Beam.localScale;
        newScale.z = Vector3.Distance(transform.position, Target.position) * OvershootPct;
        Beam.localScale = newScale;
    }

    public override void CeaseFire()
    {
        Animator.SetTrigger(DEACTIVATE_TRIGGER);
        Service.TimerManager.CreateTimer(0.7f, OnTractorRetracted, null);
        base.CeaseFire();
    }

    private void OnTractorRetracted(object cookie)
    {
        Destroy(this);
    }
}
