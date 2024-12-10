using UnityEngine;

public class RotateNpcOverTimeAction : NpcBaseAction
{
    private const float ANGLE_THRESHOLD = 1f;
    public Transform LookTarget;
    public float TargetRotation;
    public float TurnRate;

    private float angleToTarget;
    private float rightModifier;
    public override void Initiate()
    {
        TargetRotation = GetAdjustedAngle(TargetRotation);
        Service.UpdateManager.AddObserver(OnUpdate);
    }

    private void OnUpdate(float dt)
    {
        Vector2 flatFwd = new Vector2(TargetEntity.transform.forward.x, TargetEntity.transform.forward.z).normalized;
        Vector2 flatRt = new Vector2(TargetEntity.transform.right.x, TargetEntity.transform.right.z).normalized;

        if (LookTarget != null)
        {
            Vector3 vectorToNext = LookTarget.position - TargetEntity.transform.position;
            Vector2 flatVecToNext = new Vector2(vectorToNext.x, vectorToNext.z).normalized;
            rightModifier = Vector2.Dot(flatRt, flatVecToNext) > 0 ? 1f : -1f;
            angleToTarget = Vector2.Angle(flatFwd, flatVecToNext);
        }
        else
        {
            rightModifier = 1f;
            angleToTarget = TargetRotation - GetAdjustedAngle(TargetEntity.transform.eulerAngles.y);
        }

        float amountToRotate = Mathf.Min(angleToTarget, TargetEntity.NavComponent.TurnRate * dt) * rightModifier;
        TargetEntity.transform.Rotate(new Vector3(0f, amountToRotate, 0f));
    }

    private float GetAdjustedAngle(float angle)
    {
        while(angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }
}
