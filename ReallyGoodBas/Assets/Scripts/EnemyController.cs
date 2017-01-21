using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	// public variables
	public float movementSpeed = 7f;

	// components
	Rigidbody2D rb;

	// private variables
	public GameObject player;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		rb.velocity = Vector2.ClampMagnitude( new Vector2 (player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y), movementSpeed);
	}
}
