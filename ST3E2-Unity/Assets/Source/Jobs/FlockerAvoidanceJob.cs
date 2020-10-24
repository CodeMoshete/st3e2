using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct FlockerAvoidanceJob : IJob
{
    public Vector3 FlockerPosition;
    public Vector3 TargetPosition;
    public Vector3 EulerAngles;
    public float TurnSpeed;
    public float MaxPitch;
    public float AvoidanceDistance;
    public NativeArray<Vector3> Result;

    public void Execute()
    {
        Vector3 fpos = FlockerPosition;
        Vector3 bpos = TargetPosition;
        Vector3 distVec = fpos - bpos;
        float sqrDist =
            (Mathf.Pow(distVec.x, 2) + Mathf.Pow(distVec.y, 2) + Mathf.Pow(distVec.z, 2));

        if (sqrDist <= AvoidanceDistance)
        {
            Vector3 directionVector = FlockerPosition - TargetPosition;
            Vector3 orientation = EulerAngles;

            float yawDegrees =
                Mathf.DeltaAngle(orientation.y,
                (Mathf.Atan2(directionVector.x, directionVector.z) / (Mathf.PI / 180))) + 180;

            float desiredYawRotation = EulerAngles.y;

            if (yawDegrees >= 180)
                desiredYawRotation = Mathf.Min(yawDegrees, TurnSpeed);

            else if (yawDegrees < 180)
                desiredYawRotation = Mathf.Max(-yawDegrees, -TurnSpeed);

            float desiredPitchRotation = 0;

            if (FlockerPosition.y < TargetPosition.y &&
                ((EulerAngles.x > 180) ||
                (EulerAngles.x < MaxPitch)))
            {
                desiredPitchRotation = TurnSpeed / 3;
            }
            else if (FlockerPosition.y > TargetPosition.y &&
                (EulerAngles.x < 180) ||
                (EulerAngles.x > (360.0f - MaxPitch)))
            {
                desiredPitchRotation = -TurnSpeed / 3;
            }

            Result[0] = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);
        }
        else
        {
            Result[0] = Vector3.zero;
        }
    }
}
