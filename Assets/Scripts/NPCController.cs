using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

	bool interacting;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BeginInteraction() {
		//TODO: Testing
		if (!interacting) {
			PlayerController.FreezePlayer(true);
			interacting = true;
			print("Hi!  I'm an NPC!");
			PlayerController.FreezePlayer(false);
		}
	}

	public void EndInteraction() {
		interacting = false;
	}
}
