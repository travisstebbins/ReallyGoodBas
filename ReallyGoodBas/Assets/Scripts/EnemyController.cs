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
	private int color;

	void Start () {
		navMesh = GetComponent<NavMeshAgent> ();
		navMesh.updateRotation = false;
		navMesh.speed = movementSpeed;
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player.GetComponent<PlayerController>().getColor() != color) {
			navMesh.speed = 0;
			GetComponent<BoxCollider> ().enabled = false;
			switch (color) {
			case 0:
				GetComponent<SpriteRenderer> ().color = new Color (1, 0, 0, player.GetComponent<PlayerController> ().getInactiveOpacity ());
				break;
			case 1:
				GetComponent<SpriteRenderer> ().color = new Color (0, 1, 0, player.GetComponent<PlayerController> ().getInactiveOpacity ());
				break;
			case 2:
				GetComponent<SpriteRenderer> ().color = new Color (0, 0, 1, player.GetComponent<PlayerController> ().getInactiveOpacity ());
				break;
			}
		}
	}

	void Update () {
		//navMesh.stoppingDistance = stoppingDistance;
		//navMesh.destination = target.position;
		navMesh.SetDestination (player.transform.position);
	}

	public void setColor (int _color) {
		color = _color;
	}
}
