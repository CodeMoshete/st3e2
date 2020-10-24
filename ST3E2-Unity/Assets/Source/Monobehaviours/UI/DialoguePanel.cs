using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public GameObject Panel;
    public TextReveal DialogueText;
    public Image IconImage;
    public Animator Animator;
    public GameObject PingEffect;

    private bool isTransitioning;
    private bool isShowingDialogue;

	private void Start ()
    {
        Service.EventManager.AddListener(EventId.ShowDialogueText, ShowDialogueText);
        Service.EventManager.AddListener(EventId.DialogueTextDismissed, HideDialogueText);
    }

    private void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.ShowDialogueText, ShowDialogueText);
        Service.EventManager.RemoveListener(EventId.DialogueTextDismissed, HideDialogueText);
    }

    private bool ShowDialogueText(object cookie)
    {
        LinearDialogueData data = (LinearDialogueData)cookie;
        if (!isShowingDialogue && !isTransitioning)
        {
            isShowingDialogue = true;
            isTransitioning = true;
            Panel.SetActive(true);
            Animator.SetBool("IsVisible", true);
            Service.TimerManager.CreateTimer(0.5f, TransitionInComplete, null);
        }
        TriggerPing();
        IconImage.sprite = data.CharacterIcon;
        DialogueText.text = data.DialogueText;
        return true;
    }

    private void TransitionInComplete(object cookie)
    {
        isTransitioning = false;
    }

    private bool HideDialogueText(object cookie)
    {
        if (isShowingDialogue && !isTransitioning)
        {
            isShowingDialogue = false;
            isTransitioning = true;
            Animator.SetBool("IsVisible", false);
            Service.TimerManager.CreateTimer(0.5f, TransitionOutComplete, null);
        }
        return true;
    }

    private void TransitionOutComplete(object cookie)
    {
        isTransitioning = false;
        Panel.SetActive(false);
    }

    private void TriggerPing()
    {
        PingEffect.SetActive(false);
        PingEffect.SetActive(true);
    }
}
