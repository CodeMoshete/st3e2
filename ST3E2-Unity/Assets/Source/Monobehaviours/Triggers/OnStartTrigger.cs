using UnityEngine;

public class OnStartTrigger : MonoBehaviour
{
    public CustomAction NextAction;
    
    private void Start()
    {
        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }
}
