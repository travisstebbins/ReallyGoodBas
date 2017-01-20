using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// public variables
	public float maxSpeed = 10f;
	public float jumpHeight = 10f;

	// components
	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		if (Input.GetButton("Jump")) {
			rb.velocity = new Vector2 (rb.velocity.x, Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y));
		}
	}

	void FixedUpdate () {
		float moveX = Input.GetAxis ("Horizontal");
		rb.velocity = new Vector2 (moveX * maxSpeed, rb.velocity.y);
	}
}
