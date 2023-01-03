using UnityEngine;

public class NpcBaseAction : CustomAction
{
    protected CharacterEntity TargetEntity;

    public override void Initiate()
    {
        Transform currentParent = transform.parent;
        while (currentParent != null && currentParent.GetComponent<NavNode>() == null)
        {
            currentParent = transform.parent;
        }

        if (currentParent != null)
        {
            TargetEntity = currentParent.GetComponent<NavNode>().CurrentCharacter;
        }
    }
}
