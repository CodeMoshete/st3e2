public class SetSplinePathEnabledAction : CustomAction
{
    public SplineController ControllerTarget;
    public SplineInterpolator InterpolatorTarget;
    public bool IsEnabled;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        ControllerTarget.enabled = IsEnabled;
        InterpolatorTarget.enabled = IsEnabled;

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
