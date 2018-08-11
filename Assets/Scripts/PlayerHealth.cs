using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	bool isDead = false;

	float hearts = 3;
	float currentHealth = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddHeart() {
		if (!isDead) {
			hearts++;
			currentHealth++;
			UIController.UpdateHearts(hearts, currentHealth);
		}
	}

	public void TakeDamage(float damage) {
		if (!isDead) {
			currentHealth -= damage;
			print(currentHealth);
			if (currentHealth <= 0) {
				isDead = true;
				print("Oh noes!  I have died.");
				PlayerController.FreezePlayer(true);
				//TODO: Player died.
			}
		}
	}
}
