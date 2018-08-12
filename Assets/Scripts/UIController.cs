using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour {

	[SerializeField]
	Sprite fullHeart;
	[SerializeField]
	Sprite halfHeart;
	[SerializeField]
	Sprite emptyHeart;

	[SerializeField]
	Image[] Hearts;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateHearts(int total, float current) {
		int empty = total - Mathf.CeilToInt(current); //Use the ceiling, as a half heart would not count as empty.
		int full = Mathf.FloorToInt(current); //Uses floor, as half hearts don't count as full either.
		int halfHeartIndex = 7; //Out of bounds value to catch
		if (!Mathf.Floor(current).Equals(current)) { //Check for half heart
			halfHeartIndex = full; //Full is a number (1-indexed), while this pointer is 0-indexed, thus no need to add 1.
		}

		while (empty > 0) {
			Hearts[total - empty].sprite = emptyHeart;
			Hearts[total - empty].overrideSprite = emptyHeart;
			empty--;
		}

		while (total > 0) {
			Hearts[total - 1].enabled = true;
			total--;
		}

		while(full > 0) {
			Hearts[full - 1].sprite = fullHeart;
			Hearts[full - 1].overrideSprite = fullHeart;
			full--;
		}

		if(halfHeartIndex <= 5) {
			Hearts[halfHeartIndex].sprite = halfHeart;
			Hearts[halfHeartIndex].overrideSprite = halfHeart;
		}
	}
}
