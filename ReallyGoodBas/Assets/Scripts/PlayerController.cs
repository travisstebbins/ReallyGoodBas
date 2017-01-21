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

	// components
	private Rigidbody2D rb;
	private GameObject groundCheck;
	private GameObject wallCheck;
	private BoxCollider2D boxColl;
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
		rb = GetComponent<Rigidbody2D> ();
		boxColl = GetComponent<BoxCollider2D> ();
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
	}

	void Update () {
		Collider2D touchingWall = Physics2D.OverlapCircle (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
		anim.SetBool ("isWallSliding", touchingWall && !isGrounded);
		if (Input.GetButton("Jump")) {
			if (touchingWall) {
				prevWallJumpDirection = wallJumpDirection;
				wallJumpDirection = (transform.position.x < touchingWall.gameObject.transform.position.x) ? -1 : 1;
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
				rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
			}
		}
		else if (Input.GetButton("Slide") && isGrounded) {
			StartCoroutine (SlideCoroutine (slideDuration));
		}
		if (Input.GetButton("Red")) {
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
		else if (Input.GetButton("Green")) {
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
		else if (Input.GetButton("Blue")) {
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
	}

	void FixedUpdate () {
		isGrounded = Physics2D.OverlapCircle (groundCheck.transform.position, groundCheckRadius, groundLayerMask);
		Collider2D touchingWall = Physics2D.OverlapCircle (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
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
		if (touchingWall && !justWallJumped) {
			int wallSlideDirection = (transform.position.x < touchingWall.gameObject.transform.position.x) ? -1 : 1;
			if (wallSlideDirection * moveX < 0 && !isGrounded) {
				rb.velocity = new Vector2 (rb.velocity.x, -1);
			}
		}
		if (wallJumping) {
			if (!justWallJumped) {
//				if (rb.velocity.x < maxSpeed) {
//					rb.AddForce (new Vector2 (moveX * wallJumpMovementMultiplier, 0));
//				}
				rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
			}
		}
		else if (!sliding) {
			rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
		}
	}

	void Flip () {
		facingRight = !facingRight;
		Vector2 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	IEnumerator SlideCoroutine (float slideDuration) {
		boxColl.offset = new Vector2 (boxColl.offset.x, -0.84f);
		boxColl.size = new Vector2 (boxColl.size.x, 0.99f);
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveX * maxSpeed * slideSpeed, rb.velocity.y);
		sliding = true;
		yield return new WaitForSeconds (slideDuration);
		sliding = false;
		boxColl.offset = new Vector2 (boxColl.offset.x, 0.19f);
		boxColl.size = new Vector2 (boxColl.size.x, 3.29f);
	}

	IEnumerator JustWallJumpedCoroutine (float wallJumpDuration) {
		justWallJumped = true;
		rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (4f * jumpHeight * -Physics2D.gravity.y));
		rb.AddForce (new Vector2 (wallJumpDirection * wallJumpMaxForce, 0));
		yield return new WaitForSeconds (wallJumpDuration);
		justWallJumped = false;
	}
}