﻿using UnityEngine;

public class MoveNpcOverTimeAction : NpcBaseAction
{
    public Transform StartPosition;
    public Transform Destination;
    public float MoveTime;
    public CustomAction NextAction;

    private float initialMoveTime;

    public override void Initiate()
    {
        base.Initiate();
        initialMoveTime = MoveTime;
        Service.UpdateManager.AddObserver(OnUpdate);
    }

    private void OnUpdate(float dt)
    {
        MoveTime -= dt;

        if (MoveTime > 0f)
        {
            float pct = 1f - MoveTime / initialMoveTime;
            Vector3 newPos = Vector3.Lerp(StartPosition.position, Destination.position, pct);
            TargetEntity.transform.position = newPos;
        }
        else
        {
            Debug.Log("[MoveTransformOverTimeAction]: Done!");
            TargetEntity.transform.position = Destination.position;
            Service.UpdateManager.RemoveObserver(OnUpdate);

            if (NextAction != null)
            {
                NextAction.Initiate();
            }
        }
    }
}
