using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorBehavior : MonoBehaviour {

	public string DoorName = "TURBOLIFT";
	public string DoorNumber = "01";

	public Text NumberText;
	public Text NameText;

	public List<Animator> m_animators;

	// Use this for initialization
	void Start ()
	{
		if (NumberText != null)
        {
			NumberText.text = DoorNumber;
		}

		if (NameText != null)
        {
			NameText.text = DoorName;
        }
	}
	
    void OnTriggerEnter(Collider collisionObject) {
		print ("Collision Detected enter");
        if(collisionObject.tag == "Player")
		{
			for(int i = 0, count = m_animators.Count; i < count; i++)
			{
				m_animators[i].SetBool("PlayerInRange", true);
			}
		}
    }
	
	void OnTriggerExit(Collider collisionObject) {
		print ("Collision Detected exit");
        if(collisionObject.tag == "Player")
		{
			for(int i = 0, count = m_animators.Count; i < count; i++)
			{
				m_animators[i].SetBool("PlayerInRange", false);
			}
		}
    }
}
