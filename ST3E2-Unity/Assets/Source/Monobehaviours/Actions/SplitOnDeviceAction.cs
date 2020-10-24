public class SplitOnDeviceAction : CustomAction
{
    public CustomAction OnOculusQuest;
    public CustomAction OnOculusGo;

    public override void Initiate()
    {
        switch (Service.Controls.CurrentHeadset)
        {
            case HeadsetModel.OculusGo:
                if (OnOculusGo != null)
                {
                    OnOculusGo.Initiate();
                }
                break;
            case HeadsetModel.OculusQuest:
                if (OnOculusQuest != null)
                {
                    OnOculusQuest.Initiate();
                }
                break;
        }
    }
}
