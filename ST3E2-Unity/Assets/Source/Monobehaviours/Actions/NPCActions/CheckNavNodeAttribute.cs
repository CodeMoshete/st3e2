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
                IfTrue.Initiate();
            }
            else if (!attribute.Value && IfFalse)
            {
                IfFalse.Initiate();
            }
        }
        else if (IfNotExists != null)
        {
            IfNotExists.Initiate();
        }
    }
}
