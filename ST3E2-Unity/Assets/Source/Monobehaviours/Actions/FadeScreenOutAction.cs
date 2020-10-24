using UnityEngine;

public class FadeScreenOutAction : CustomAction
{
    public OVRScreenFade ScreenFade;
    public CustomAction OnComplete;
    public float Delay;
    public float Duration;
    private float totalDuration;
    private bool isInitialized;

    public override void Initiate()
    {
        Debug.Log("FADE OUT: " + gameObject.name);
        if (ScreenFade != null)
        {
            if (Duration > 0f)
            {
                totalDuration = Duration;
                isInitialized = true;
            }
            else
            {
                ScreenFade.SetFadeLevel(1f);
                if (OnComplete != null)
                {
                    OnComplete.Initiate();
                }
            }
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

            float pct = totalDuration > 0f ? 1f - (Duration / totalDuration) : 1f;
            ScreenFade.SetFadeLevel(pct);
        }
    }
}
