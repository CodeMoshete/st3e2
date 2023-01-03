public class SetNavNodeAttribute : CustomAction
{
    public NavNode TargetNode;
    public string AttributeName;
    public bool AttributeValue;
    public CustomAction NextAction;

    public override void Initiate()
    {
        TargetNode.SetAttribute(AttributeName, AttributeValue);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
