using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileHitTrigger : MonoBehaviour
{
    public int NumHits;
    public CustomAction OnHitsSustained;

    private int initialNumHits;
    private Collider localCollider;

    // Start is called before the first frame update
    private void Start()
    {
        initialNumHits = NumHits;
        localCollider = GetComponent<Collider>();
        localCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WeaponBase>() != null)
        {
            NumHits--;
            if (NumHits <= 0)
            {
                OnHitsSustained.Initiate();
                localCollider.enabled = false;
            }
        }
    }

    public void Reset()
    {
        NumHits = initialNumHits;
        localCollider.enabled = true;
    }
}
