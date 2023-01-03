public class ReleaseCurrentCharacterAction : CustomAction
{
    public NavNode TargetNode;
    public CustomAction NextAction;

    public override void Initiate()
    {
        TargetNode.CurrentCharacter = null;

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
