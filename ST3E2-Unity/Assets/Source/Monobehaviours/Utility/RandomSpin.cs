using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    public float MaxSpinRate;
    private Vector3 spinRate;

    private void Start()
    {
        spinRate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        spinRate = spinRate.normalized * MaxSpinRate;
    }

    private void Update()
    {
        transform.Rotate(spinRate * Time.deltaTime);
    }
}
