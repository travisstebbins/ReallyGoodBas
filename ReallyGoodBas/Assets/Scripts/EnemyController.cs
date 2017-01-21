using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

	// public variables
	public float movementSpeed = 4f;
	//public float stoppingDistance = 0.1f;

	// components
	NavMeshAgent navMesh;

	// private variables
	private GameObject player;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		navMesh = GetComponent<NavMeshAgent> ();
		navMesh.updateRotation = false;
	}

	void Update () {
		navMesh.speed = movementSpeed;
		//navMesh.stoppingDistance = stoppingDistance;
		//navMesh.destination = target.position;
		navMesh.SetDestination (player.transform.position);
	}

//	void FixedUpdate () {
//		rb.velocity = Vector2.ClampMagnitude( new Vector2 (player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y), movementSpeed);
//	}
}
