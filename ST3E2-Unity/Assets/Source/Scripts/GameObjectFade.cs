using UnityEngine;
using System.Collections;

public class GameObjectFade : CustomAction
{
	public enum FadeOperation
	{
		FadeOut,
		FadeIn
	}

	private enum FadeState
	{
		Idle,
		Fading
	}

	private FadeState fadeState;
	public FadeOperation fadeType;
	public GameObject ObjectToFade;
	public float FadeDuration;
	private float fadeCurrentTime;

	private float fromVal;
	private float toVal;

	public override void Initiate()
	{
		fadeCurrentTime = FadeDuration;
		fadeState = FadeState.Fading;

		fromVal = fadeType == FadeOperation.FadeIn ? 1f : 0f;
		toVal = fadeType == FadeOperation.FadeIn ? 0f : 1f;
	}

	// Update is called once per frame
	void Update () {
		if(fadeState == FadeState.Fading)
		{
			fadeCurrentTime -= Time.deltaTime;
			Color objCol = ObjectToFade.GetComponent<Renderer>().material.color;
			objCol.a = Mathf.Lerp(fromVal, toVal, (1f - (fadeCurrentTime / FadeDuration)));
			ObjectToFade.GetComponent<Renderer>().material.color = objCol;

			if(fadeCurrentTime <= 0f)
			{
				fadeState = FadeState.Idle;
			}
		}
	}
}
