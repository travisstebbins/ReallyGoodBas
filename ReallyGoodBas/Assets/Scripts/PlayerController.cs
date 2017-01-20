using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Policy;

public class PlayerController : MonoBehaviour {

	// public variables
	public float maxSpeed = 10f;
	public float jumpHeight = 5f;
	public float groundCheckRadius = 0.1f;
	public float wallCheckRadius = 0.3f;
	public float wallJumpMaxForce = 100f;
	public float wallJumpDeceleration = 0.5f;

	// components
	private Rigidbody2D rb;
	private GameObject groundCheck;
	private GameObject wallCheck;

	// private variables
	private int color = 1;
	private bool isGrounded = false;
	private bool wallJumping = false;
	private int wallJumpDirection = -1;
	private int prevWallJumpDirection = 1;
	private float wallJumpForce = 10f;
	private LayerMask groundLayerMask;
	private LayerMask jumpWallLayerMask;
	private GameObject[] redObjects;
	private GameObject[] greenObjects;
	private GameObject[] blueObjects;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
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
		if (isGrounded) {
			wallJumping = false;
		}
		if (Input.GetButton("Jump")) {
			Collider2D touchingWall = Physics2D.OverlapCircle (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
			if (touchingWall) {
				prevWallJumpDirection = wallJumpDirection;
				wallJumpDirection = (transform.position.x < touchingWall.gameObject.transform.position.x) ? -1 : 1;
				if (!wallJumping) {
					prevWallJumpDirection = wallJumpDirection * -1;
				}
				wallJumping = true;
				wallJumpForce = wallJumpMaxForce;
				if (wallJumpDirection == prevWallJumpDirection * -1) {
					rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
				}
				else {
					rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y);
				}
			}
			else if (isGrounded) {
				rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
			}
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
		if (Input.GetButton("Green")) {
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
		if (Input.GetButton("Blue")) {
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
		Collider2D jumpWall = Physics2D.OverlapCircle (wallCheck.transform.position, wallCheckRadius, jumpWallLayerMask);
		float moveX = Input.GetAxis ("Horizontal");
		if (wallJumping) {
			if (!jumpWall) {
				rb.velocity = new Vector2 (moveX * maxSpeed + (wallJumpDirection * wallJumpForce), rb.velocity.y);
			}
			else if (wallJumpDirection == prevWallJumpDirection * -1) {
				rb.velocity = new Vector2 (wallJumpDirection * wallJumpForce, rb.velocity.y);
			}
//			else {
//				rb.velocity = new Vector2 (wallJumpDirection * wallJumpForce, rb.velocity.y);
//			}
			wallJumpForce -= wallJumpDeceleration;
			if (wallJumpForce < 0) {
				wallJumpForce = 0;
			}
		}
		else if (!jumpWall || isGrounded) {
			rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
		}
	}
}