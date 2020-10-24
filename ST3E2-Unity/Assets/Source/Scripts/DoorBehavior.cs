using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorBehavior : MonoBehaviour {

	public string DoorName = "TURBOLIFT";
	public string DoorNumber = "01";

	public GameObject[] DoorParts;
	private List<Animator> m_animators;

	// Use this for initialization
	void Start ()
	{
		m_animators = new List<Animator>();

		for(int i = 0, count = DoorParts.Length; i < count; i++)
		{
			GameObject door = DoorParts[i];
			FindGameObject(door, "NumberText").GetComponent<Text>().text = DoorNumber;
			FindGameObject(door, "TitleText").GetComponent<Text>().text = DoorName;
			m_animators.Add(door.GetComponent<Animator>());
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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

	private GameObject FindGameObject(GameObject parent, string name)
	{
		GameObject returnObject = null;
		for(int i = 0, count = parent.transform.childCount; i < count; i++)
		{
			Transform child = parent.transform.GetChild(i);
			if(child.name == name)
			{
				returnObject = child.gameObject;
			}

			if(child.childCount > 0)
			{
				returnObject = FindGameObject(child.gameObject, name);
			}

			if(returnObject != null)
			{
				break;
			}
		}
		return returnObject;
	}
}
