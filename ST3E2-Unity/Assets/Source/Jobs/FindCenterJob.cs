using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct FindCenterJob : IJob
{
    public NativeArray<Vector3> FlockerPositions;
    public NativeArray<Vector3> Result;

    public void Execute()
    {
        int flockSize = FlockerPositions.Length;
        Vector3 Center = Vector3.zero;

        for (int i = 0; i < flockSize; ++i)
        {
            Center += FlockerPositions[i];
        }

        Center /= flockSize;

        Result[0] = Center;
    }
}
