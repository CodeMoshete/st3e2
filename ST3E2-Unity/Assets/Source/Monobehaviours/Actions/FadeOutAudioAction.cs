using UnityEngine;

public class FadeOutAudioAction : CustomAction
{
    public AudioSource AudioSource;
    public float FadeOutTime;

    private float initialVolume;
    private float currentTime;

    public override void Initiate()
    {
        initialVolume = AudioSource.volume;
        Service.UpdateManager.AddObserver(OnUpdate);
        currentTime = 0f;
    }

    private void OnUpdate(float dt)
    {
        currentTime += dt;
        float pct = Mathf.Max(1f - (currentTime / FadeOutTime), 0f);
        AudioSource.volume = pct * initialVolume;
        if (currentTime > FadeOutTime)
        {
            AudioSource.gameObject.SetActive(false);
            AudioSource.volume = initialVolume;
            Service.UpdateManager.RemoveObserver(OnUpdate);
        }
    }
}
