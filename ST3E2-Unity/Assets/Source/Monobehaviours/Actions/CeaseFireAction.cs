using UnityEngine;

public class CeaseFireAction : CustomAction
{
    public FireWeaponBaseAction WeaponAction;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        base.Initiate();
        if (WeaponAction != null)
        {
            WeaponAction.CeaseFire();
        }
        else
        {
            Debug.LogError("No weapon deceted to cease fire of!");
        }

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
