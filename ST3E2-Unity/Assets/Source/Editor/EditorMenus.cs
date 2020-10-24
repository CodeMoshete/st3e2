using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMenus : Editor
{
    [MenuItem("LongView/Align Sockets")]
    public static void AlignSockets()
    {
        HallSegment[] segments = GameObject.FindObjectsOfType<HallSegment>();
        List<List<HallSegment>> OrderedSegmentGroups = new List<List<HallSegment>>();
        for (int i = 0, count = segments.Length; i < count; ++i)
        {
            HallSegment segment = segments[i];
            if (segment.IsAnchorSegment)
            {
                List<HallSegment> currentGroup = new List<HallSegment>();
                for (int j = 0, count2 = segment.Sides.Count; j < count2; ++j)
                {
                    HallSegment nextSegment =
                        segment.Sides[j].ConnectedPiece.GetComponent<HallSegment>();

                    ProcessSegment(ref currentGroup, nextSegment, segment);
                }
            }
        }
    }

    private static void ProcessSegment(
        ref List<HallSegment> currentSegments, HallSegment thisSegment, HallSegment lastSegment)
    {
        //currentSegments.Add(thisSegment);
        Align(thisSegment, lastSegment);

        for (int i = 0, count = thisSegment.Sides.Count; i < count; ++i)
        {
            HallSegment nextSegment = 
                thisSegment.Sides[i].ConnectedPiece.GetComponent<HallSegment>();

            if (nextSegment != null && 
                nextSegment != lastSegment && 
                !currentSegments.Contains(nextSegment))
            {
                ProcessSegment(ref currentSegments, nextSegment, thisSegment);
            }
        }
    }

    private static void Align(HallSegment thisSegment, HallSegment lastSegment)
    {
        Debug.Log("Aligning " + thisSegment.name);

        Transform thisSegmentSocket = GetSocketPosition(thisSegment, lastSegment);
        Transform lastSegmentSocket = GetSocketPosition(lastSegment, thisSegment);

        Vector3 thisSocketLocalPos = 
            thisSegmentSocket.transform.position - thisSegment.transform.position;

        thisSegment.transform.position = 
            lastSegmentSocket.transform.position - thisSocketLocalPos;
    }

    private static Transform GetSocketPosition(HallSegment sourceSegment, HallSegment destinationSegment)
    {
        Transform socketPos = null;
        for (int i = 0, count = sourceSegment.Sides.Count; i < count; ++i)
        {
            HallSegment targetSegment = 
                sourceSegment.Sides[i].ConnectedPiece.GetComponent<HallSegment>();

            if (targetSegment != null && targetSegment == destinationSegment)
            {
                socketPos = sourceSegment.Sides[i].Socket.transform;
            }
        }
        return socketPos;
    }
}
