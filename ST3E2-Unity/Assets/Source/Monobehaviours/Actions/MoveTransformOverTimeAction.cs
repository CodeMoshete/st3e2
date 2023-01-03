using UnityEngine;

public class MoveTransformOverTimeAction : CustomAction
{
    public Transform TargetTransform;
    public Transform StartPosition;
    public Transform Destination;
    public float MoveTime;
    public CustomAction NextAction;

    private float initialMoveTime;

    public override void Initiate()
    {
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
            TargetTransform.position = newPos;
        }
        else
        {
            Debug.Log("[MoveTransformOverTimeAction]: Done!");
            TargetTransform.position = Destination.position;
            Service.UpdateManager.RemoveObserver(OnUpdate);

            if (NextAction != null)
            {
                NextAction.Initiate();
            }
        }
    }
}
