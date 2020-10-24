using UnityEngine;

public enum CloakState
{
    Cloak,
    Decloak
}

public class CloakAction : CustomAction
{
    private const string PROP_METALLIC = "_Metallic";
    private const string PROP_GLOSSINESS = "_Glossiness";

    public GameObject Target;
    public float Duration;
    public CloakState EndState;
    public CustomAction OnComplete;
    public float DefaultMetallic;
    public float DefaultGloss;

    private float totalDuration;
    private Material targetMaterial;
    private bool isInitialized;

    private float targetMetallic;
    private float sourceMetallic;
    private float targetSmoothness;
    private float sourceSmoothness;

    public override void Initiate()
    {
        if (!isInitialized && Target != null && Duration > 0f)
        {
            totalDuration = Duration;
            targetMaterial = Target.GetComponent<Renderer>().material;

            targetMetallic = EndState == CloakState.Cloak ? 1f : DefaultMetallic;
            sourceMetallic = EndState == CloakState.Cloak ? DefaultMetallic : 1f;
            targetSmoothness = EndState == CloakState.Cloak ? 1f : DefaultGloss;
            sourceSmoothness = EndState == CloakState.Cloak ? DefaultGloss : 1f;

            isInitialized = true;
        }
    }

    private void Update()
    {
        if (isInitialized)
        {
            float dt = Time.deltaTime;
            float pct =  Mathf.Max(0f, 1f - (Duration / totalDuration));

            Duration -= dt;
            targetMaterial.SetFloat(
                PROP_METALLIC, Mathf.Lerp(sourceMetallic, targetMetallic, pct));

            targetMaterial.SetFloat(
                PROP_GLOSSINESS, Mathf.Lerp(sourceSmoothness, targetSmoothness, pct));

            if (Duration <= 0f)
            {
                isInitialized = false;

                if (OnComplete != null)
                {
                    OnComplete.Initiate();
                }
            }
        }
    }
}
