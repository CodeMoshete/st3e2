using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueOption
{
    public string OptionText;
    public CustomAction OnSelected;

    public DialogueOption(string optionTxt, CustomAction onSelected)
    {
        OptionText = optionTxt;
        OnSelected = onSelected;
    }
}

public class ShowBranchingDialogueAction : CustomAction
{
    public string Prompt;
    public Sprite ProfileImage;
    public float ShowDelay;
    public float TimeLimit;
    public CustomAction OnTimeRanOut;
    public List<DialogueOption> Options;

    public override void Initiate()
    {
        if (ShowDelay > 0f)
        {
            Service.TimerManager.CreateTimer(ShowDelay, OnShowReady, null);
        }
        else
        {
            Service.EventManager.SendEvent(EventId.ShowChoiceDialogue, this);
        }
    }

    private void OnShowReady(object cookie)
    {
        Service.EventManager.SendEvent(EventId.ShowChoiceDialogue, this);
    }

    public void OnOptionSelected(int optionIndex)
    {
        Service.EventManager.SendEvent(EventId.ChoiceDialogueDismissed, null);
        DialogueOption selectedOption = Options[optionIndex];
        if (selectedOption.OnSelected != null)
        {
            selectedOption.OnSelected.Initiate();
        }
    }

    public void OnTimeOut()
    {
        Service.EventManager.SendEvent(EventId.ChoiceDialogueDismissed, null);
        if (OnTimeRanOut != null)
        {
            OnTimeRanOut.Initiate();
        }
    }
}
