using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Policy;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

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
	private int color = 1;
	private bool isGrounded = false;
	private bool wallJumping = false;
	private int wallJumpDirection = -1;
	private int prevWallJumpDirection = 1;
	private bool justWallJumped = false;
	private bool sliding = false;
	private bool facingRight = true;
	private LayerMask groundLayerMask;
	private LayerMask jumpWallLayerMask;
	private GameObject[] redObjects;
	private GameObject[] greenObjects;
	private GameObject[] blueObjects;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		boxColl = GetComponent<BoxCollider> ();
		anim = GetComponent<Animator> ();
		groundCheck = GameObject.FindGameObjectWithTag ("PlayerGroundCheck");
		wallCheck = GameObject.FindGameObjectWithTag ("PlayerWallCheck");
		groundLayerMask = LayerMask.GetMask ("Ground");
		jumpWallLayerMask = LayerMask.GetMask ("JumpWall");
		redObjects = GameObject.FindGameObjectsWithTag ("Red");
		greenObjects = GameObject.FindGameObjectsWithTag ("Green");
		blueObjects = GameObject.FindGameObjectsWithTag ("Blue");
		for (int i = 0; i < greenObjects.Length; ++i) {
			greenObjects [i].SetActive (false);
		}
		for (int i = 0; i < blueObjects.Length; ++i) {
			blueObjects [i].SetActive (false);
		}
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
				Debug.Log ("wallJumpDirection: " + wallJumpDirection + ", prevWallJumpDirection: " + prevWallJumpDirection);
				wallJumping = true;
				if (wallJumpDirection == prevWallJumpDirection * -1) {
					if (wallJumpDirection * Input.GetAxis ("Horizontal") < 0) {
						Debug.Log ("Wall Jump!");
						StartCoroutine (JustWallJumpedCoroutine (wallJumpDuration));
					}
				}
			}
			else if (isGrounded) {
				rb.velocity = new Vector3 (rb.velocity.x, 0, Mathf.Sqrt (2f * jumpHeight * -Physics.gravity.z));
			}
		}
		else if (Input.GetButton("Slide") && isGrounded) {
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
		isGrounded = (Physics.OverlapSphere (groundCheck.transform.position, groundCheckRadius, groundLayerMask)).Length > 0;
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
			Debug.Log ("holding onto wall");
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

	void Flip () {
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void SetRed () {
		color = 1;
		for (int i = 0; i < redObjects.Length; ++i) {
			redObjects [i].SetActive (true);
		}
		for (int i = 0; i < greenObjects.Length; ++i) {
			greenObjects [i].SetActive (false);
		}
		for (int i = 0; i < blueObjects.Length; ++i) {
			blueObjects [i].SetActive (false);
		}
	}

	void SetGreen () {
		color = 2;
		for (int i = 0; i < redObjects.Length; ++i) {
			redObjects [i].SetActive (false);
		}
		for (int i = 0; i < greenObjects.Length; ++i) {
			greenObjects [i].SetActive (true);
		}
		for (int i = 0; i < blueObjects.Length; ++i) {
			blueObjects [i].SetActive (false);
		}
	}

	void SetBlue () {
		color = 3;
		for (int i = 0; i < redObjects.Length; ++i) {
			redObjects [i].SetActive (false);
		}
		for (int i = 0; i < greenObjects.Length; ++i) {
			greenObjects [i].SetActive (false);
		}
		for (int i = 0; i < blueObjects.Length; ++i) {
			blueObjects [i].SetActive (true);
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
}