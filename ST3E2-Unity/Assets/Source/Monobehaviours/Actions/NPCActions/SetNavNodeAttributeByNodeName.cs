public class SetNavNodeAttributeByNodeName : CustomAction
{
    public string TargetNetworkName;
    public string TargetNodeName;
    public string AttributeName;
    public bool AttributeValue;
    public CustomAction NextAction;

    public override void Initiate()
    {
        NavNode TargetNode =
            Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(TargetNetworkName).GetNodeByName(TargetNodeName);

        TargetNode.SetAttribute(AttributeName, AttributeValue);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
