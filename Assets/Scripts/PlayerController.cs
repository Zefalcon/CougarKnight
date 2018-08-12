using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	LayerMask groundLayer;

	[SerializeField]
	bool isGrounded() {
		Vector2 position = transform.position;
		Vector2 down = Vector2.down;
		float dist = 0.1f;

		Debug.DrawRay(position, down, Color.green);
		RaycastHit2D hit = Physics2D.Raycast(position, down, dist, groundLayer);
		if (hit.collider != null) {
			return true;
		}

		return false;
	}
	[SerializeField]
	int numJumps = 0;
	[SerializeField]
	bool canDoubleJump;
	float speed = 5;
	Animator anim;
	Rigidbody2D rb;

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
		//Ground check for double jump
		//if (isGrounded()) {
		//	onFirstJump = false;
		//}

		//TODO: Testing
		if ((Input.GetKeyDown(KeyCode.Q))) {
			FreezePlayer(false);
		}
		if ((Input.GetKeyDown(KeyCode.O))) {
			canDoubleJump = true;
		}

		//Freeze check
		if (playerFrozen) {
			anim.SetFloat("Speed", 0);
			anim.SetBool("SetJumping", false);
			anim.SetBool("SetDigging", false);
			anim.SetBool("SetAttacking", false);
			isRunning = false;
			isAttacking = false;
			isDigging = false;
			//TODO: Any checks that require a frozen player
			return;
		}

		//Interact
		if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && atNPC) {
			NPC.BeginInteraction();
			//TODO: Get NPC collider, do thing, unfreeze once complete
		}

		//Movement
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
		else if (isGrounded()) {
			anim.SetFloat("Speed", 0);
			isRunning = false;
		}

		//Jump
		if (Input.GetKeyDown(KeyCode.Space) && !isDigging && !isAttacking) {
			if (/*isGrounded() ||*/ numJumps < 1) { //Either on the ground, or fell off a platform and hasn't jumped yet
				anim.SetBool("SetJumping", true);
				rb.velocity = new Vector2(rb.velocity.x, 0);
				rb.AddForce(new Vector2(0, 260));
				numJumps = 1;
			}
			else if (canDoubleJump && numJumps<=1) { //In the air and has only jumped once, if they are able to double jump
				anim.SetBool("SetDoubleJump", true);
				numJumps = 2; //Disables jumping after this
				rb.velocity = new Vector2(rb.velocity.x, 0);
				rb.AddForce(new Vector2(0, 260));
			}
		}

		//Dig
		if (Input.GetKey(KeyCode.P) && !isAttacking && isGrounded()) {
			isDigging = true;
			anim.SetBool("SetDigging", true);
			//TODO: Dig stuff, freeze player motion
		}

		//Attack
		if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !isDigging) {
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
		else if (coll.gameObject.tag == "Enemy") {
			if (isAttacking) {
				coll.gameObject.GetComponent<DogController>().Kill();
				Physics2D.IgnoreCollision(coll, GetComponent<BoxCollider2D>());
			}
			else {
				if (coll.gameObject.GetComponent<DogController>().isDead) {
					Physics2D.IgnoreCollision(coll, GetComponent<BoxCollider2D>());
				}
				else {
					GetComponent<PlayerHealth>().TakeDamage(0.5f);
				}
			}
		}
	}

	private void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.tag == "Enemy") {
			if (isAttacking) {
				coll.gameObject.GetComponent<DogController>().Kill();
				Physics2D.IgnoreCollision(coll, GetComponent<BoxCollider2D>());
			}
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
			anim.SetBool("SetJumping", false);
			anim.SetBool("SetDoubleJump", false);
			if (isGrounded()) {
				numJumps = 0;
			}
		}
		else {
			Physics2D.IgnoreCollision(coll.collider, GetComponent<BoxCollider2D>());
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
		//Backup for weird circumstances
		if (collision.collider.gameObject.tag == "Floor") {
			if (isGrounded()) {
				numJumps = 0;
			}
			anim.SetBool("SetJumping", false);
			anim.SetBool("SetDoubleJump", false);
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
