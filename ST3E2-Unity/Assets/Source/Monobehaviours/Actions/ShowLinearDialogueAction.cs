using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinearDialogueData
{
    public Sprite CharacterIcon;
    public string DialogueText;
    public float ShowDelay;
    public float ShowDuration;
}

public class ShowLinearDialogueAction : CustomAction
{
    public bool LockMovement;
    public List<LinearDialogueData> Dialogues;
    public CustomAction OnStart;
    public CustomAction OnComplete;

    private LinearDialogueData currentDialogueStep;

    private int dialogueIndex;

    public override void Initiate()
    {
        dialogueIndex = 0;
        if (dialogueIndex < Dialogues.Count - 1)
        {
            LinearDialogueData nextDialogue = Dialogues[0];
            if (nextDialogue.ShowDelay > 0f)
            {
                Service.TimerManager.CreateTimer(nextDialogue.ShowDelay, ShowNextDialogue, null);
            }
            else
            {
                ShowNextDialogue(null);
            }
        }
        else
        {
            ShowNextDialogue(null);
        }

        if (OnStart != null)
        {
            OnStart.Initiate();
        }
    }

    private void ShowNextDialogue(object cookie)
    {
        bool isLastDialogue = dialogueIndex == Dialogues.Count - 1;

        if (dialogueIndex == 0)
        {
            if (LockMovement)
            {
                Service.Controls.SetTouchObserver(OnTouch);
            }
            Service.Controls.SetTriggerObserver(OnTrigger);
        }

        currentDialogueStep = Dialogues[dialogueIndex];
        if (currentDialogueStep.ShowDuration > 0f)
        {
            Service.TimerManager.CreateTimer(currentDialogueStep.ShowDuration, OnTimeExpired, null);
        }

        Service.EventManager.SendEvent(EventId.ShowDialogueText, Dialogues[dialogueIndex]);
        dialogueIndex++;
    }

    private void OnTouch(TouchpadUpdate update)
    {
        // Intentionally empty.
    }

    private void OnTrigger(TriggerUpdate update)
    {
        if (update.TriggerClicked && currentDialogueStep.ShowDuration <= 0)
        {
            CheckNextStartTime();
        }
    }

    private void OnTimeExpired(object cookie)
    {
        CheckNextStartTime();
    }

    private void CheckNextStartTime()
    {
        if (dialogueIndex < Dialogues.Count)
        {
            LinearDialogueData nextDialogue = Dialogues[dialogueIndex];
            if (nextDialogue.ShowDelay > 0f)
            {
                Service.EventManager.SendEvent(EventId.DialogueTextDismissed, null);
                Service.TimerManager.CreateTimer(nextDialogue.ShowDelay, TriggerContinue, null);
                return;
            }
        }

        TriggerContinue(null);
    }

    private void TriggerContinue(object cookie)
    {
        bool isDone = dialogueIndex == Dialogues.Count;

        if (isDone)
        {
            Service.Controls.RemoveTouchObserver(OnTouch);
            Service.Controls.RemoveTriggerObserver(OnTrigger);

            if (!(OnComplete is ShowLinearDialogueAction))
            {
                Service.EventManager.SendEvent(EventId.DialogueTextDismissed, null);
            }

            if (OnComplete != null)
            {
                OnComplete.Initiate();
            }
        }
        else
        {
            ShowNextDialogue(null);
        }
    }
}
