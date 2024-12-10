using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorBehavior : MonoBehaviour {

	public string DoorName = "TURBOLIFT";
	public string DoorNumber = "01";

	public Text NumberText;
	public Text NameText;

	public List<Animator> m_animators;
    public CustomAction OnOpen;
    public CustomAction OnClose;

	private List<Collider> collidingObjects = new List<Collider>();

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
        if(collisionObject.tag == "Player" || collisionObject.tag == "NPC")
        {
			collidingObjects.Add(collisionObject);

			for(int i = 0, count = m_animators.Count; i < count; i++)
			{
				m_animators[i].SetBool("PlayerInRange", true);
			}

            if (OnOpen != null)
            {
                OnOpen.Initiate();
            }
		}
    }
	
	void OnTriggerExit(Collider collisionObject) {
		print ("Collision Detected exit");
        if(collisionObject.tag == "Player" || collisionObject.tag == "NPC")
		{
			collidingObjects.Remove(collisionObject);

			if (collidingObjects.Count > 0)
			{
				return;
			}

			for(int i = 0, count = m_animators.Count; i < count; i++)
			{
				m_animators[i].SetBool("PlayerInRange", false);
			}

            if (OnClose != null)
            {
                OnClose.Initiate();
            }
        }
    }
}
