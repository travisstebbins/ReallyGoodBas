using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

	// public variables
	public float maxSpeed = 10f;
	public float jumpHeight = 10f;
	public float groundCheckRadius = 0.5f;

	// components
	private Rigidbody2D rb;
	private GameObject groundCheck;

	// private variables
	private bool isGrounded = false;
	private LayerMask groundLayerMask;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		groundCheck = GameObject.FindGameObjectWithTag ("PlayerGroundCheck");
		groundLayerMask = LayerMask.GetMask ("Ground");
	}

	void Update () {
		if (isGrounded && Input.GetButton("Jump")) {
			rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
		}
	}

	void FixedUpdate () {
		isGrounded = Physics2D.OverlapCircle (groundCheck.transform.position, groundCheckRadius, groundLayerMask);
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
	}
}
