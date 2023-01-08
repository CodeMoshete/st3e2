public class SetUpNavWorldAction : CustomAction
{
    public NavWorldID NavWorld;
    public string NetworkName;
    public CustomAction NextAction;

    public override void Initiate()
    {
        Service.NavWorldManager.SetNavWorldAndNetwork(NavWorld, NetworkName);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
