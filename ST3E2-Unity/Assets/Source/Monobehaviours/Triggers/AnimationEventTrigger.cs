using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationEventTrigger : MonoBehaviour {

	[System.Serializable]
	public struct AnimationTriggerParams
	{
		public GameObject Parent;
		public string TriggerAnimationName;
		public string AnimationName;
	}

	[System.Serializable]
	public struct AnimationCustomActionParams
	{
		public GameObject Parent;
		public string TriggerAnimationName;
	}

	public List<AnimationTriggerParams> TriggerParams;
	public List<AnimationCustomActionParams> ActionParams;

	public void OnAnimationEvent(string eventId)
	{
		for(int i = 0, count = TriggerParams.Count; i < count; i++)
		{
			if(string.Equals(eventId, TriggerParams[i].TriggerAnimationName))
			{
				TriggerParams[i].Parent.GetComponent<Animator>().SetTrigger(TriggerParams[i].AnimationName);
			}
		}

		for(int i = 0, count = ActionParams.Count; i < count; i++)
		{
			if(string.Equals(eventId, ActionParams[i].TriggerAnimationName))
			{
				ActionParams[i].Parent.GetComponent<CustomAction>().Initiate();
			}
		}
	}
}
