using UnityEngine;

public class AlignNpcWithNodeAction : NpcBaseAction
{
    public Transform TargetNode;
    public bool AlignPosition;
    public bool AlignRotation;
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        if (AlignPosition)
        {
            TargetEntity.transform.position = TargetNode.position;
        }

        if (AlignRotation)
        {
            Vector3 currentRotation = TargetEntity.transform.eulerAngles;
            currentRotation.y = TargetNode.eulerAngles.y;
            TargetEntity.transform.eulerAngles = currentRotation;
        }

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
