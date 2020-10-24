using UnityEngine;

public class SetPlayerControllerActive : CustomAction
{
    public GameObject OVRPlayerController;
    public bool Active;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        CharacterController charController = OVRPlayerController.GetComponent<CharacterController>();
        OVRPlayerController playerController = OVRPlayerController.GetComponent<OVRPlayerController>();
        charController.enabled = Active;
        playerController.enabled = Active;

        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
