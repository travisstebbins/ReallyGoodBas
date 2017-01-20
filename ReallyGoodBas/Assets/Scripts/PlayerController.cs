using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Policy;

public class PlayerController : MonoBehaviour {

	// public variables
	public float maxSpeed = 10f;
	public float jumpHeight = 5f;
	public float groundCheckRadius = 0.5f;

	// components
	private Rigidbody2D rb;
	private GameObject groundCheck;

	// private variables
	private int color = 1;
	private bool isGrounded = false;
	private LayerMask groundLayerMask;
	private GameObject[] redObjects;
	private GameObject[] greenObjects;
	private GameObject[] blueObjects;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		groundCheck = GameObject.FindGameObjectWithTag ("PlayerGroundCheck");
		groundLayerMask = LayerMask.GetMask ("Ground");
		redObjects = GameObject.FindGameObjectsWithTag ("Red");
		greenObjects = GameObject.FindGameObjectsWithTag ("Green");
		blueObjects = GameObject.FindGameObjectsWithTag ("Blue");
	}

	void Update () {
		if (isGrounded && Input.GetButton("Jump")) {
			rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
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
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
	}
}
