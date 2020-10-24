using UnityEngine;

public class RandomScale : MonoBehaviour
{
    public float MaxScale;
    public float MinScale;
    public bool UniformScale;

    private void Start()
    {
        Vector3 randScale = Vector3.zero;
        if (UniformScale)
        {
            float scaleVal = Random.Range(MinScale, MaxScale);
            randScale = new Vector3(scaleVal, scaleVal, scaleVal);
        }
        else
        {
            randScale = new Vector3(
                Random.Range(MinScale, MaxScale), 
                Random.Range(MinScale, MaxScale), 
                Random.Range(MinScale, MaxScale));
        }

        transform.localScale = randScale;
    }
}
