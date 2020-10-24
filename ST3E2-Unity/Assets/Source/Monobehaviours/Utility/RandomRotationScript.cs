using UnityEngine;

public class RandomRotationScript : MonoBehaviour
{
    private void OnEnable()
    {
        transform.eulerAngles = new Vector3(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );
    }
}
