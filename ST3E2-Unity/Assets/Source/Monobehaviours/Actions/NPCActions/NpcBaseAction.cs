using UnityEngine;

public class NpcBaseAction : CustomAction
{
    protected CharacterEntity TargetEntity;
    protected NavNode ParentNode;

    public override void Initiate()
    {
        Transform currentParent = transform.parent;
        while (currentParent != null)
        {
            NavNode navNode = currentParent.GetComponent<NavNode>();
            if (navNode != null)
            {
                ParentNode = currentParent.GetComponent<NavNode>();
                TargetEntity = ParentNode.CurrentCharacter;
                return;
            }

            FindNpcAction findNpcAction = currentParent.GetComponent<FindNpcAction>();
            if (findNpcAction != null)
            {
                TargetEntity = findNpcAction.GetTargetEntity();
                return;
            }

            currentParent = currentParent.parent;
        }
    }
}
