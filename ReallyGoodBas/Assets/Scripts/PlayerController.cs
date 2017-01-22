using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Policy;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using System.Linq;

public class PlayerController : MonoBehaviour {

	// public variables
	public float maxSpeed = 10f;
	public float jumpHeight = 5f;
	public float groundCheckRadius = 0.1f;
	public float wallCheckRadius = 0.3f;
	public float wallJumpMaxForce = 500f;
	public float wallJumpDuration = 0.5f;
	public float wallJumpMovementMultiplier = 200f;
	public float slideSpeed = 2f;
	public float slideDuration = 0.4f;
	public float inactiveOpacity = 0.25f;
	public bool automaticCycle = false;
	public float cycleSpeed = 2f;
	public bool cycleRed = false;
	public bool cycleGreen = false;
	public bool cycleBlue = false;

	// components
	private Rigidbody rb;
	private GameObject groundCheck;
	private GameObject wallCheck;
	private BoxCollider boxColl;
	private Animator anim;

	// private variables
	private int color = 0;
	private bool isGrounded = false;
	private bool wallJumping = false;
	private int wallJumpDirection = -1;
	private int prevWallJumpDirection = 1;
	private bool justWallJumped = false;
	private bool sliding = false;
	private bool facingRight = true;
	private LayerMask groundLayerMask;
	private LayerMask jumpWallLayerMask;
	private List<GameObject> redObjects;
	private List<GameObject> greenObjects;
	private List<GameObject> blueObjects;
	private List<GameObject> enemies;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		boxColl = GetComponent<BoxCollider> ();
		anim = GetComponent<Animator> ();
		groundLayerMask = LayerMask.GetMask ("Ground");
		jumpWallLayerMask = LayerMask.GetMask ("JumpWall");
		redObjects = GameObject.FindGameObjectsWithTag ("Red").ToList();
		greenObjects = GameObject.FindGameObjectsWithTag ("Green").ToList();
		blueObjects = GameObject.FindGameObjectsWithTag ("Blue").ToList();
		Transform[] playerCheckers = GetComponentsInChildren<Transform> ();
		for (int i = 0; i < playerCheckers.Length; ++i) {
			if (playerCheckers[i].gameObject.CompareTag("PlayerGroundCheck")) {
				groundCheck = playerCheckers [i].gameObject;
			}
			else if (playerCheckers[i].gameObject.CompareTag("PlayerWallCheck")) {
				wallCheck = playerCheckers [i].gameObject;
			}
		}
		SetRed ();
		if (automaticCycle) {
			StartCoroutine (AutomaticCycleCoroutine (cycleSpeed));
		}
	}

	void Update () {
		Collider[] touchingWall = Physics.OverlapSphere (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
		anim.SetBool ("isWallSliding", (touchingWall.Length > 0) && !isGrounded);
		if (Input.GetButton("Jump")) {
			if (touchingWall.Length > 0) {
				prevWallJumpDirection = wallJumpDirection;
				wallJumpDirection = (transform.position.x < touchingWall[0].gameObject.transform.position.x) ? -1 : 1;
				if (!wallJumping) {
					prevWallJumpDirection = wallJumpDirection * -1;
				}
				wallJumping = true;
				if (wallJumpDirection == prevWallJumpDirection * -1) {
					if (wallJumpDirection * Input.GetAxis ("Horizontal") < 0) {
						StartCoroutine (JustWallJumpedCoroutine (wallJumpDuration));
					}
				}
			}
			else if (isGrounded) {
				rb.velocity = new Vector3 (rb.velocity.x, 0, Mathf.Sqrt (2f * jumpHeight * -Physics.gravity.z));
			}
		}
		else if (Input.GetButton("Slide") && isGrounded && Input.GetAxis("Horizontal") != 0) {
			anim.SetTrigger ("slide");
			StartCoroutine (SlideCoroutine (slideDuration));
		}
		if (!automaticCycle) {
			if (Input.GetButton ("Red")) {
				SetRed ();
			} else if (Input.GetButton ("Green")) {
				SetGreen ();
			} else if (Input.GetButton ("Blue")) {
				SetBlue ();
			}
		}
	}

	void FixedUpdate () {
		Collider[] groundObjects = Physics.OverlapSphere (groundCheck.transform.position, groundCheckRadius, groundLayerMask);
		isGrounded = groundObjects.Length > 0;
		Collider[] touchingWall = Physics.OverlapSphere (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
		anim.SetBool ("isJumping", !isGrounded);
		if (isGrounded) {
			anim.SetBool ("isWallSliding", false);
			wallJumping = false;
		}
		float moveX = Input.GetAxis ("Horizontal");
		if (moveX < 0) {
			anim.SetBool ("isRunning", true);
			if (facingRight) {
				Flip ();
			}
		}
		else if (moveX > 0) {
			anim.SetBool ("isRunning", true);
			if (!facingRight) {
				Flip ();
			}
		}
		else {
			anim.SetBool ("isRunning", false);
		}
		if (touchingWall.Length > 0 && !justWallJumped) {
			int wallSlideDirection = (transform.position.x < touchingWall[0].gameObject.transform.position.x) ? -1 : 1;
			if (wallSlideDirection * moveX < 0 && !isGrounded) {
				rb.velocity = new Vector3 (rb.velocity.x, 0, -1);
			}
		}
		if (wallJumping) {
			if (!justWallJumped) {
				rb.velocity = new Vector3 (moveX * maxSpeed, 0, rb.velocity.z);
			}
		}
		else if (!sliding) {
			rb.velocity = new Vector3 (moveX * maxSpeed, 0, rb.velocity.z);
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.CompareTag("Spike")) {
			KillPlayer ();
		}
		if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
			KillPlayer ();
		}
	}

	void OnTriggerEnter (Collider coll) {
		if (coll.gameObject.CompareTag("PowerUp")) {
			Debug.Log ("Player triggered power-up");
			KillAllEnemies ();
		}
	}

	void Flip () {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		facingRight = !facingRight;
	}

	void SetRed () {
		color = 0;
		for (int i = 0; i < redObjects.Count; ++i) {
			GameObject redObject = redObjects [i].gameObject;
			if (redObject.layer == LayerMask.NameToLayer("Enemy")) {
				redObject.GetComponent<BoxCollider> ().enabled = true;
				redObject.GetComponent<NavMeshAgent> ().speed = redObject.GetComponent<EnemyController>().movementSpeed;
			}
			else {
				redObject.GetComponent<BoxCollider> ().enabled = true;
				redObject.GetComponent<NavMeshObstacle> ().enabled = true;
			}
			redObject.GetComponent<SpriteRenderer> ().color = new Color(1, 0, 0, 1);
		}
		for (int i = 0; i < greenObjects.Count; ++i) {
			GameObject greenObject = greenObjects [i].gameObject;
			if (greenObject.layer == LayerMask.NameToLayer("Enemy")) {
				greenObject.GetComponent<BoxCollider> ().enabled = false;
				greenObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				greenObject.GetComponent<BoxCollider> ().enabled = false;
				greenObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			greenObject.GetComponent<SpriteRenderer> ().color = new Color(0, 1, 0, inactiveOpacity);
		}
		for (int i = 0; i < blueObjects.Count; ++i) {
			GameObject blueObject = blueObjects [i].gameObject;
			if (blueObject.layer == LayerMask.NameToLayer("Enemy")) {
				blueObject.GetComponent<BoxCollider> ().enabled = false;
				blueObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				blueObject.GetComponent<BoxCollider> ().enabled = false;
				blueObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			blueObject.GetComponent<SpriteRenderer> ().color = new Color(0, 0, 1, inactiveOpacity);
		}
	}

	void SetGreen () {
		color = 1;
		for (int i = 0; i < redObjects.Count; ++i) {
			GameObject redObject = redObjects [i];
			if (redObject.layer == LayerMask.NameToLayer("Enemy")) {
				redObject.GetComponent<BoxCollider> ().enabled = false;
				redObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				redObject.GetComponent<BoxCollider> ().enabled = false;
				redObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			redObject.GetComponent<SpriteRenderer> ().color = new Color(1, 0, 0, inactiveOpacity);
		}
		for (int i = 0; i < greenObjects.Count; ++i) {
			GameObject greenObject = greenObjects [i];
			if (greenObject.layer == LayerMask.NameToLayer("Enemy")) {
				greenObject.GetComponent<BoxCollider> ().enabled = true;
				greenObject.GetComponent<NavMeshAgent> ().speed = greenObject.GetComponent<EnemyController> ().movementSpeed;
			}
			else {
				greenObject.GetComponent<BoxCollider> ().enabled = true;
				greenObject.GetComponent<NavMeshObstacle> ().enabled = true;
			}
			greenObject.GetComponent<SpriteRenderer> ().color = new Color(0, 1, 0, 1);
		}
		for (int i = 0; i < blueObjects.Count; ++i) {
			GameObject blueObject = blueObjects [i];
			if (blueObject.layer == LayerMask.NameToLayer("Enemy")) {
				blueObject.GetComponent<BoxCollider> ().enabled = false;
				blueObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				blueObject.GetComponent<BoxCollider> ().enabled = false;
				blueObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			blueObject.GetComponent<SpriteRenderer> ().color = new Color(0, 0, 1, inactiveOpacity);
		}
	}

	void SetBlue () {
		color = 2;
		for (int i = 0; i < redObjects.Count; ++i) {
			GameObject redObject = redObjects [i];
			if (redObject.layer == LayerMask.NameToLayer("Enemy")) {
				redObject.GetComponent<BoxCollider> ().enabled = false;
				redObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				redObject.GetComponent<BoxCollider> ().enabled = false;
				redObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			redObject.GetComponent<SpriteRenderer> ().color = new Color(1, 0, 0, inactiveOpacity);
		}
		for (int i = 0; i < greenObjects.Count; ++i) {
			GameObject greenObject = greenObjects [i];
			if (greenObject.layer == LayerMask.NameToLayer("Enemy")) {
				greenObject.GetComponent<BoxCollider> ().enabled = false;
				greenObject.GetComponent<NavMeshAgent> ().speed = 0;
			}
			else {
				greenObject.GetComponent<BoxCollider> ().enabled = false;
				greenObject.GetComponent<NavMeshObstacle> ().enabled = false;
			}
			greenObject.GetComponent<SpriteRenderer> ().color = new Color(0, 1, 0, inactiveOpacity);
		}
		for (int i = 0; i < blueObjects.Count; ++i) {
			GameObject blueObject = blueObjects [i];
			if (blueObject.layer == LayerMask.NameToLayer("Enemy")) {
				blueObject.GetComponent<BoxCollider> ().enabled = true;
				blueObject.GetComponent<NavMeshAgent> ().speed = blueObject.GetComponent<EnemyController> ().movementSpeed;
			}
			else {
				blueObject.GetComponent<BoxCollider> ().enabled = true;
				blueObject.GetComponent<NavMeshObstacle> ().enabled = true;
			}
			blueObject.GetComponent<SpriteRenderer> ().color = new Color(0, 0, 1, 1);
		}
	}

	void KillPlayer () {
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	void KillAllEnemies () {
		for (int i = 0; i < redObjects.Count; ++i) {
			if (redObjects [i].layer == LayerMask.NameToLayer ("Enemy")) {
				Destroy (redObjects [i].gameObject);
				redObjects.Remove (redObjects [i]);
			}
		}
		for (int i = 0; i < greenObjects.Count; ++i) {
			if (greenObjects [i].layer == LayerMask.NameToLayer ("Enemy")) {
				Destroy (greenObjects [i].gameObject);
				greenObjects.Remove (greenObjects [i]);
			}
		}
		for (int i = 0; i < blueObjects.Count; ++i) {
			if (blueObjects [i].layer == LayerMask.NameToLayer ("Enemy")) {
				Destroy (blueObjects [i].gameObject);
				blueObjects.Remove (blueObjects [i]);
			}
		}
	}

	IEnumerator SlideCoroutine (float slideDuration) {
		boxColl.center = new Vector3 (boxColl.center.x, -0.31f, boxColl.center.z);
		boxColl.size = new Vector3 (boxColl.size.x, 1.72f, boxColl.size.z);
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector3 (moveX * maxSpeed * slideSpeed, 0, rb.velocity.z);
		sliding = true;
		yield return new WaitForSeconds (slideDuration);
		sliding = false;
		boxColl.center = new Vector3 (boxColl.center.x, 0.35f, boxColl.center.z);
		boxColl.size = new Vector3 (boxColl.size.x, 3.1f, boxColl.size.z);
		anim.ResetTrigger ("slide");
	}

	IEnumerator JustWallJumpedCoroutine (float wallJumpDuration) {
		justWallJumped = true;
		rb.velocity = new Vector3 (rb.velocity.x, rb.velocity.y, Mathf.Sqrt (4f * jumpHeight * -Physics.gravity.z));
		rb.AddForce (new Vector3 (wallJumpDirection * wallJumpMaxForce, 0, 0));
		yield return new WaitForSeconds (wallJumpDuration);
		justWallJumped = false;
	}

	IEnumerator AutomaticCycleCoroutine (float cycleSpeed) {
		if (cycleRed) {
			color = 1;
			SetRed ();
		}
		else if (cycleGreen) {
			color = 2;
			SetGreen ();
		}
		else if (cycleBlue) {
			color = 3;
			SetBlue ();
		}
		while (true) {
			yield return new WaitForSeconds (cycleSpeed);
			switch (color) {
			case 1:
				if (cycleGreen) {
					color = 2;
					SetGreen ();
				} else if (cycleBlue) {
					color = 3;
					SetBlue ();
				}
				break;
			case 2:
				if (cycleBlue) {
					color = 3;
					SetBlue ();
				} else if (cycleRed) {
					color = 1;
					SetRed ();
				}
				break;
			case 3:
				if (cycleRed) {
					color = 1;
					SetRed ();
				} else if (cycleGreen) {
					color = 2;
					SetGreen ();
				}
				break;
			}
		}
	}

	public void AddColoredObject (int color, GameObject obj) {
		switch (color) {
		case 0:
			redObjects.Add(obj);
			break;
		case 1:
			greenObjects.Add (obj);
			break;
		case 2:
			blueObjects.Add (obj);
			break;
		}
	}

	public int getColor () {
		return color;
	}

	public float getInactiveOpacity () {
		return inactiveOpacity;
	}
}