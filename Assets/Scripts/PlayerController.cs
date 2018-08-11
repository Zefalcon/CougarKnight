using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	bool isGrounded;
	bool onFirstJump;
	bool canDoubleJump;
	float speed = 5;
	Animator anim;
	Rigidbody2D rb;

	[SerializeField]
	bool isJumping;
	[SerializeField]
	bool isRunning;
	[SerializeField]
	bool isAttacking;
	[SerializeField]
	bool isDigging;

	string currentDirection = "right";

	static bool playerFrozen;
	[SerializeField]
	bool atNPC;
	[SerializeField]
	NPCController NPC;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		//Freeze check
		//TODO: Testing
		if ((Input.GetKeyDown(KeyCode.Q))) {
			FreezePlayer(false);
		}
		if ((Input.GetKeyDown(KeyCode.O))) {
			canDoubleJump = true;
		}

		if (playerFrozen) {
			anim.SetFloat("Speed", 0);
			anim.SetBool("SetJumping", false);
			anim.SetBool("SetDigging", false);
			anim.SetBool("SetAttacking", false);
			isJumping = false;
			isRunning = false;
			isAttacking = false;
			isDigging = false;
			//TODO: Any checks that require a frozen player
			return;
		}

		//Button Check
		if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && atNPC) {
			//Interact
			NPC.BeginInteraction();
			//TODO: Get NPC collider, do thing, unfreeze once complete
		}

		if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !isDigging) {
			//Go left
			anim.SetFloat("Speed", speed);
			isRunning = true;
			changeDirection("left");
			transform.Translate(Vector3.left * speed * Time.deltaTime);
		}
		else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !isDigging) {
			//Go right
			anim.SetFloat("Speed", speed);
			isRunning = true;
			changeDirection("right");
			transform.Translate(Vector3.right * speed * Time.deltaTime);
		}
		else if (isGrounded) {
			anim.SetFloat("Speed", 0);
			isRunning = false;
		}


		if (Input.GetKeyDown(KeyCode.Space) && !isDigging && !isAttacking && (isGrounded || (canDoubleJump && onFirstJump))) {
			//Jump
			isGrounded = false;
			anim.SetBool("SetJumping", true);
			rb.AddForce(new Vector2(0, 250));
			isJumping = true;
			if (!onFirstJump) {
				onFirstJump = true;
			}
			else {
				anim.SetBool("SetDoubleJump", true);
				onFirstJump = false;
			}
		}

		if (Input.GetKey(KeyCode.P) && !isAttacking && isGrounded) {
			//Dig
			isDigging = true;
			anim.SetBool("SetDigging", true);
			//TODO: Dig stuff, freeze player motion
		}

		if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !isDigging) {
			//Attack
			isAttacking = true;
			anim.SetBool("SetAttacking", true);
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

	private void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "NPC") {
			atNPC = true;
			NPC = coll.gameObject.GetComponent<NPCController>();
		}
	}

	private void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.tag == "NPC") {
			//TODO: Testing
			NPC.EndInteraction();
			NPC = null;
			atNPC = false;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Floor") {
			isGrounded = true;
			isJumping = false;
			onFirstJump = false;
			anim.SetBool("SetJumping", false);
			anim.SetBool("SetDoubleJump", false);
		}
		else if (coll.gameObject.tag == "Enemy") {
			if (isAttacking) {
				coll.gameObject.GetComponent<DogController>().Kill();
				Physics2D.IgnoreCollision(coll.collider, GetComponent<BoxCollider2D>());
			}
			else {
				if (coll.gameObject.GetComponent<DogController>().isDead) {
					Physics2D.IgnoreCollision(coll.collider, GetComponent<BoxCollider2D>());
				}
				else {
					GetComponent<PlayerHealth>().TakeDamage(0.5f);
					print("Ow!");
				}
			}
		}
		else {
			Physics2D.IgnoreCollision(coll.collider, GetComponent<BoxCollider2D>());
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
		if (collision.collider.gameObject.tag == "Enemy") {
			if (isAttacking) {
				collision.collider.gameObject.GetComponent<DogController>().Kill();
				Physics2D.IgnoreCollision(collision.collider, GetComponent<BoxCollider2D>());
			}
		}
	}

	public static void FreezePlayer(bool freeze) {
		playerFrozen = freeze;
	}

	void StopAttack() {
		isAttacking = false;
		anim.SetBool("SetAttacking", false);
	}

	void StopDig() {
		isDigging = false;
		anim.SetBool("SetDigging", false);
	}

}
