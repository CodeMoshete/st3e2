using System;
using UnityEngine;
using UnityEngine.UI;

public class TextReveal : Text
{
    public Action OnShowComplete;
    private const float CHARS_PER_SEC = 80;
    private float totalDuration;
    private float currentTime;
    private string content;

    public override string text
    {
        get
        {
            return base.text;
        }

        set
        {
            base.text = value;
            Initialize();
        }
    }

    private void Initialize()
    {
        content = base.text;
        totalDuration = content.Length / CHARS_PER_SEC;
        currentTime = totalDuration;
        base.text = string.Empty;
    }

    public void Update()
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
            float pct = (totalDuration - currentTime) / totalDuration;
            int charsToShow = 
                Mathf.Min(content.Length, Mathf.CeilToInt(pct * content.Length));

            string textToShow = content.Substring(0, charsToShow);
            base.text = textToShow;

            if (pct >= 1f && OnShowComplete != null)
            {
                OnShowComplete();
                OnShowComplete = null;
            }
        }
    }
}
