using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private GameObject player;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update () {
		gameObject.transform.position = new Vector3 (player.gameObject.transform.position.x, player.gameObject.transform.position.y, -10);
	}
}
