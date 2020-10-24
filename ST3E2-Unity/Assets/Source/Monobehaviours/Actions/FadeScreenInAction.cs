using UnityEngine;

public class FadeScreenInAction : CustomAction
{
    public OVRScreenFade ScreenFade;
    public CustomAction OnStart;
    public CustomAction OnComplete;
    public float Delay;
    public float Duration;
    private float totalDuration;
    private bool isInitialized;

    public override void Initiate()
    {
        if (ScreenFade != null)
        {
            totalDuration = Duration;
            isInitialized = true;
        }

        if (OnStart != null)
        {
            OnStart.Initiate();
        }
    }

    public void Update()
    {
        if (isInitialized)
        {
            float dt = Time.deltaTime;
            if (Delay > 0)
            {
                Delay -= dt;
                return;
            }

            Duration -= dt;
            if (Duration <= 0f)
            {
                Duration = 0f;
                isInitialized = false;
                if (OnComplete != null)
                {
                    OnComplete.Initiate();
                }
            }

            float pct = Duration / totalDuration;
            ScreenFade.SetFadeLevel(pct);
        }
    }
}
