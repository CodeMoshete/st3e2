﻿public class ReleaseCurrentCharacterAction : NpcBaseAction
{
    public CustomAction NextAction;

    public override void Initiate()
    {
        base.Initiate();

        ParentNode.ReleaseCurrentCharacter();

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
