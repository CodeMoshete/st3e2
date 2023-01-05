using UnityEngine;

public class MoveNpcOverTimeAction : NpcBaseAction
{
    public Transform StartPosition;
    public Transform Destination;
    public bool EaseIn;
    public bool EaseOut;
    public float MoveTime;
    public CustomAction OnStartAction;
    public CustomAction OnFinishAction;

    private float initialMoveTime;

    public override void Initiate()
    {
        base.Initiate();
        initialMoveTime = MoveTime;
        Service.UpdateManager.AddObserver(OnUpdate);

        if (OnStartAction != null)
        {
            OnStartAction.Initiate();
        }
    }

    void ApplyEase(ref float pct)
    {
        if (EaseIn && EaseOut)
        {
            if (pct > 0.5f)
            {
                pct = -(2f * Mathf.Pow(pct - 1f, 2f)) + 1;
            }
            else
            {
                pct = 2f * Mathf.Pow(pct, 2f);
            }
        }
        else if (EaseIn)
        {
            pct = pct * pct;
        }
        else if (EaseOut)
        {
            pct = -(Mathf.Pow(pct - 1f, 2f)) + 1f;
        }
    }

    private void OnUpdate(float dt)
    {
        MoveTime -= dt;

        if (MoveTime > 0f)
        {
            float pct = 1f - MoveTime / initialMoveTime;
            ApplyEase(ref pct);
            Vector3 newPos = Vector3.Lerp(StartPosition.position, Destination.position, pct);
            TargetEntity.transform.position = newPos;
        }
        else
        {
            Debug.Log("[MoveTransformOverTimeAction]: Done!");
            TargetEntity.transform.position = Destination.position;
            Service.UpdateManager.RemoveObserver(OnUpdate);

            if (OnFinishAction != null)
            {
                MoveTime = initialMoveTime;
                OnFinishAction.Initiate();
            }
        }
    }
}
