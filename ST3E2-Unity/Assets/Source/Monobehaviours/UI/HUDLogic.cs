using UnityEngine;
using UnityEngine.UI;

public class HUDLogic : MonoBehaviour
{
    public GameObject TriggerPressContainer;

    public Text TextPromptContainer;
    public Animator TextPromptAnimator;
    public GameObject DebugPanel;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowTriggerPrompt, ShowTriggerPress);
        Service.EventManager.AddListener(EventId.ShowPromptText, ShowPromptText);
        Service.EventManager.AddListener(EventId.HidePromptText, HidePromptTextFromEvent);
        Service.EventManager.AddListener(EventId.DebugToggleConsole, ToggleDebugConsole);
    }

    public void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.ShowTriggerPrompt, ShowTriggerPress);
        Service.EventManager.RemoveListener(EventId.ShowPromptText, ShowPromptText);
        Service.EventManager.RemoveListener(EventId.HidePromptText, HidePromptTextFromEvent);
        Service.EventManager.RemoveListener(EventId.DebugToggleConsole, ToggleDebugConsole);
    }

    public bool ShowTriggerPress(object cookie)
    {
        bool show = (bool)cookie;
        TriggerPressContainer.SetActive(show);
        return false;
    }

    public bool ShowPromptText(object cookie)
    {
        PromptTextActionData promptData = (PromptTextActionData)cookie;
        TextPromptContainer.text = promptData.Prompt;
        TextPromptAnimator.SetBool("IsVisible", true);
        if (promptData.Duration > 0)
        {
            Service.TimerManager.CreateTimer(promptData.Duration, HidePromptTextFromTimer, null);
        }
        return true;
    }

    public bool HidePromptTextFromEvent(object cookie)
    {
        TextPromptAnimator.SetBool("IsVisible", false);
        return true;
    }

    public void HidePromptTextFromTimer(object cookie)
    {
        TextPromptAnimator.SetBool("IsVisible", false);
    }

    public bool ToggleDebugConsole(object cookie)
    {
        DebugPanel.SetActive((bool)cookie);
        return true;
    }
}
