using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour {

	Animator anim;

	[SerializeField]
	Transform leftStop;
	[SerializeField]
	Transform rightStop;
	string currentDirection = "right";
	float speed = 3f;

	private bool _isDead;
	public bool isDead {
		get { return _isDead; }
		private set { _isDead = value; }
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: Testing
		if (!isDead && Input.GetKeyDown(KeyCode.Y)) {
			anim.SetBool("isDead", true);
			isDead = true;
		}

		if (!isDead) {
			//Walk left for a while, then walk right
			if (transform.position.x <= leftStop.position.x && currentDirection == "left") {
				//Stop going left
				changeDirection("right");
				transform.Translate(Vector3.right * speed * Time.deltaTime);
			}
			else if (transform.position.x >= rightStop.position.x && currentDirection == "right") {
				//Stop going right
				changeDirection("left");
				transform.Translate(Vector3.left * speed * Time.deltaTime);
			}
			else if (currentDirection == "left") {
				//Go left
				transform.Translate(Vector3.left * speed * Time.deltaTime);
			}
			else if (currentDirection == "right") {
				//Go right
				transform.Translate(Vector3.right * speed * Time.deltaTime);
			}
		}
	}

	void changeDirection(string direction) {
		if (currentDirection != direction) {
			if (direction == "right") {
				GetComponent<SpriteRenderer>().flipX = false;
				currentDirection = "right";
			}
			else if (direction == "left") {
				GetComponent<SpriteRenderer>().flipX = true;
				currentDirection = "left";
			}
		}
	}

	public void Kill() {
		if (!isDead) {
			anim.SetBool("isDead", true);
			isDead = true;
			StartCoroutine("WaitForDeath");
		}
	}

	IEnumerator WaitForDeath() {
		yield return new WaitForSeconds(1.0f);
		Destroy(this.gameObject);
	}
}
