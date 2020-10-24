using UnityEngine;

public enum StatEvaluation
{
    GreaterThan,
    LessThan,
    EqualTo,
    NotEqualTo
}

public class EvaluatePlayerStatAction : CustomAction
{
    public string StatName;
    public StatEvaluation EvaluationType;
    public int TestValue;
    public CustomAction OnTrue;
    public CustomAction OnFalse;

    public override void Initiate()
    {
        bool evalResult = false;
        int currentStatValue = Service.PlayerData.GetStat(StatName);
        switch(EvaluationType)
        {
            case StatEvaluation.GreaterThan:
                evalResult = currentStatValue > TestValue;
                break;
            case StatEvaluation.LessThan:
                evalResult = currentStatValue < TestValue;
                break;
            case StatEvaluation.EqualTo:
                evalResult = currentStatValue == TestValue;
                break;
            case StatEvaluation.NotEqualTo:
                evalResult = currentStatValue != TestValue;
                break;
        }

        Debug.Log("Evaluated " + StatName + " as " + currentStatValue + 
            " " + EvaluationType.ToString() + ": " + evalResult);

        if (evalResult && OnTrue != null)
        {
            OnTrue.Initiate();
        }
        else if (!evalResult && OnFalse != null)
        {
            OnFalse.Initiate();
        }
    }
}
