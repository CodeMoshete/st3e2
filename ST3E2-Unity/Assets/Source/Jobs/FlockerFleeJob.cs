using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct FlockerFleeJob : IJob
{
    public Vector3 FlockerPosition;
    public Vector3 EulerAngles;
    public Vector3 TargetPosition;
    public float TurnSpeed;
    public float MaxPitch;

    public NativeArray<Vector3> Result;

    public void Execute()
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

        Vector3 retVec = new Vector3(desiredPitchRotation, desiredYawRotation, 0.0f);

        Result[0] = retVec;
    }
}