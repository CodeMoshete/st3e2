using UnityEngine;

public class NpcBaseAction : CustomAction
{
    protected CharacterEntity TargetEntity;
    protected NavNode ParentNode;

    public override void Initiate()
    {
        Transform currentParent = transform.parent;
        while (currentParent != null && currentParent.GetComponent<NavNode>() == null)
        {
            currentParent = currentParent.parent;
        }

        if (currentParent != null)
        {
            ParentNode = currentParent.GetComponent<NavNode>();
            TargetEntity = ParentNode.CurrentCharacter;
        }
    }
}
