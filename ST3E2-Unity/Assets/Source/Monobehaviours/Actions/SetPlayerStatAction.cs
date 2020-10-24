public enum SetStatOperation
{
    Set,
    Increment,
    Decrement
}

public class SetPlayerStatAction : CustomAction
{
    public SetStatOperation Operation;
    public string StatName;
    public int Value;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        switch(Operation)
        {
            case SetStatOperation.Set:
                Service.PlayerData.SetStat(StatName, Value);
                break;
            case SetStatOperation.Increment:
                Service.PlayerData.UpdateStat(StatName, Value);
                break;
            case SetStatOperation.Decrement:
                Service.PlayerData.UpdateStat(StatName, -Value);
                break;
        }

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
