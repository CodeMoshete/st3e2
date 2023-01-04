using UnityEngine;

public class CheckNavNodeAttribute : CustomAction
{
    public NavNode TargetNode;
    public string AttributeName;
    public CustomAction IfTrue;
    public CustomAction IfFalse;
    public CustomAction IfNotExists;

    public override void Initiate()
    {
        NavNodeAttribute attribute = TargetNode.GetAttribute(AttributeName);
        if (attribute != null)
        {
            if (attribute.Value && IfTrue)
            {
                Debug.Log(AttributeName + " is TRUE");
                IfTrue.Initiate();
            }
            else if (!attribute.Value && IfFalse)
            {
                Debug.Log(AttributeName + " is FALSE");
                IfFalse.Initiate();
            }
        }
        else if (IfNotExists != null)
        {
            Debug.Log(AttributeName + " is NOT FOUND");
            IfNotExists.Initiate();
        }
    }
}
