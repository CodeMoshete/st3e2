using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiDialoguePanel : MonoBehaviour
{
    private const string DEFAULT_OPTION_TEXT_GO = "<Select an option using the thumb-pad...>";
    private const string DEFAULT_OPTION_TEXT_QUEST = "<Select an option using the right joystick and gun trigger...>";
    private const float SELECT_FILL_TIME = 1f;

    private readonly Color BUTTON_DEACTIVATED = new Color(1f, 0.698f, 0f, 0f);
    private readonly Color BUTTON_DESELECTED = new Color(1f, 0.698f, 0f, 0.34f);
    private readonly Color BUTTON_SELECTED = new Color(1f, 0.698f, 0f, 1f);

    private readonly Vector2 OPTION_1 = Vector2.up;
    private readonly Vector2 OPTION_2 = Vector2.right;
    private readonly Vector2 OPTION_3 = Vector2.down;
    private readonly Vector2 OPTION_4 = Vector2.left;

    public GameObject Panel;
    public Animator Animator;
    public Image ProfileImage;
    public GameObject PingEffect;
    public TextReveal PromptText;
    public List<Image> OptionImages;
    public Text ResponseText;
    public Image SelectionFill;
    public GameObject Timer;
    public Image TimerFill;
    public GameObject TimerTutorialText;

    private bool isTransitioning;
    private bool isShowingDialogue;

    private bool isTimed;
    private bool isCountingDown;
    private float timeLeft;
    private float totalTime;

    private float optionSelectPct;
    private int currentOptionIndex;
    private bool isOptionSelectingTouch;
    private bool isOptionSelectingTrigger;
    private bool isOptionSelecting
    {
        get
        {
            return isOptionSelectingTouch || isOptionSelectingTrigger;
        }
    }

    private ShowBranchingDialogueAction actionData;

    public void Start()
    {
        Service.EventManager.AddListener(EventId.ShowChoiceDialogue, ShowMultiDialogue);
        Service.EventManager.AddListener(EventId.ChoiceDialogueDismissed, HideMultiDialogue);
    }

    public void OnDestroy()
    {
        Service.EventManager.RemoveListener(EventId.ShowChoiceDialogue, ShowMultiDialogue);
        Service.EventManager.RemoveListener(EventId.ChoiceDialogueDismissed, HideMultiDialogue);
    }

    private bool ShowMultiDialogue(object cookie)
    {
        if (!isShowingDialogue && !isTransitioning)
        {
            Service.Controls.SetTouchObserver(OnTouchUpdate);
            Service.Controls.SetTriggerObserver(OnTriggerUpdate);
            Service.UpdateManager.AddObserver(OnUpdate);

            actionData = (ShowBranchingDialogueAction)cookie;
            Panel.SetActive(true);

            ProfileImage.sprite = actionData.ProfileImage;
            PromptText.text = actionData.Prompt;
            PromptText.OnShowComplete = PromptTextDisplayed;
            TriggerPing();
            Animator.SetBool("IsVisible", true);

            isTimed = actionData.TimeLimit > 0f;
            isCountingDown = false;
            if (isTimed)
            {
                timeLeft = actionData.TimeLimit;
                totalTime = timeLeft;
            }
            Timer.SetActive(isTimed);

            for (int i = 0, count = OptionImages.Count; i < count; ++i)
            {
                OptionImages[i].color = BUTTON_DEACTIVATED;
            }

            isShowingDialogue = true;
            isTransitioning = true;
            Service.TimerManager.CreateTimer(0.5f, TransitionInComplete, null);
        }
        return false;
    }

    private void PromptTextDisplayed()
    {
        Service.TimerManager.CreateTimer(1f, ShowChoice, null);
    }

    private void ShowChoice(object cookie)
    {
        Animator.SetBool("IsPlayerChoiceVisible", true);

        if (isTimed)
        {
            Service.TimerManager.CreateTimer(1f, BeginCountdown, null);
        }
    }

    private void BeginCountdown(object cookie)
    {
        isCountingDown = true;
    }

    private bool HideMultiDialogue(object cookie)
    {
        Service.Controls.RemoveTouchObserver(OnTouchUpdate);
        Service.Controls.RemoveTriggerObserver(OnTriggerUpdate);
        Service.UpdateManager.RemoveObserver(OnUpdate);
        isShowingDialogue = false;
        Animator.SetBool("IsVisible", false);
        Service.TimerManager.CreateTimer(0.5f, TransitionOutComplete, null);
        return false;
    }

    private void OnTouchUpdate(TouchpadUpdate update)
    {
        int prevOptionIndex = currentOptionIndex;

        if (update.TouchpadPosition == Vector2.zero && currentOptionIndex >= 0)
        {
            currentOptionIndex = -1;
            SetOptionHighlighted(currentOptionIndex);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_1) > 0.75f && 
            currentOptionIndex != 0)
        {
            SetOptionHighlighted(0);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_2) > 0.75f && 
            currentOptionIndex != 1)
        {
            SetOptionHighlighted(1);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_3) > 0.75f && 
            currentOptionIndex != 2)
        {
            SetOptionHighlighted(2);
        }
        else if (Vector2.Dot(update.TouchpadPosition.normalized, OPTION_4) > 0.75f && 
            currentOptionIndex != 3)
        {
            SetOptionHighlighted(3);
        }

        isOptionSelectingTouch = 
            update.TouchpadPressState && 
            currentOptionIndex != -1 && 
            Service.Controls.CurrentHeadset != HeadsetModel.OculusQuest;

        if (!isOptionSelecting || currentOptionIndex != prevOptionIndex)
        {
            optionSelectPct = 0f;
        }
    }

    private void OnUpdate(float dt)
    {
        if (isOptionSelecting)
        {
            optionSelectPct += dt / SELECT_FILL_TIME;

            if (optionSelectPct >= 1f)
            {
                Service.UpdateManager.RemoveObserver(OnUpdate);
                SelectOption();
            }
        }
        SelectionFill.fillAmount = optionSelectPct;

        if (isCountingDown)
        {
            timeLeft -= dt;
            TimerFill.fillAmount = Mathf.Max(0f, timeLeft / totalTime);
            if (timeLeft <= 0f && !isOptionSelecting)
            {
                Service.UpdateManager.RemoveObserver(OnUpdate);
                actionData.OnTimeOut();
            }
        }
    }

    private void SetOptionHighlighted(int optionIndex)
    {
        int numOptions = actionData.Options.Count;

        if (optionIndex >= 0 && optionIndex < numOptions)
        {
            currentOptionIndex = optionIndex;
            ResponseText.text = actionData.Options[optionIndex].OptionText;
        }
        else if (optionIndex == -1)
        {
            string optionText = Service.Controls.CurrentHeadset == HeadsetModel.OculusQuest ? 
                DEFAULT_OPTION_TEXT_QUEST : DEFAULT_OPTION_TEXT_GO;
            ResponseText.text = optionText;
        }

        for (int i = 0; i < numOptions; ++i)
        {
            if (i == optionIndex)
            {
                OptionImages[i].color = BUTTON_SELECTED;
            }
            else
            {
                OptionImages[i].color = BUTTON_DESELECTED;
            }
        }
    }

    private void OnTriggerUpdate(TriggerUpdate update)
    {
        isOptionSelectingTrigger = update.TriggerPressState && currentOptionIndex != -1;
        if (!isOptionSelecting)
        {
            optionSelectPct = 0f;
        }
    }

    private void SelectOption()
    {
        actionData.OnOptionSelected(currentOptionIndex);
    }

    private void TransitionInComplete(object cookie)
    {
        isTransitioning = false;
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
