using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// public variables
	public float scrollDistanceXProportion = 0.7f;
	public float scrollDistanceYProportion = 0.7f;
	public float defaultY = 10f;

	// private variables2d
	private GameObject player;
	private Vector3 startingPos;
	private float scrollDistanceX;
	private float scrollDistanceY;

	void Awake () {
		Time.timeScale = 1;
	}
		
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		startingPos = transform.position;
		scrollDistanceX = scrollDistanceXProportion * (16.0f/9.0f) * this.GetComponent<Camera>().orthographicSize;
		scrollDistanceY = scrollDistanceYProportion * this.GetComponent<Camera>().orthographicSize;
		transform.position = new Vector3 (player.transform.position.x, defaultY, player.transform.position.z);
	}

	void Update () {
		if (player.transform.position.x > transform.position.x + (this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) {
			transform.position = new Vector3 (player.transform.position.x - (float)(this.GetComponent<Camera> ().orthographicSize * (16.0 / 9.0) - scrollDistanceX), defaultY, transform.position.z);
		}
		else if (player.transform.position.x < transform.position.x - (this.GetComponent<Camera>().orthographicSize * (16.0/9.0)) + scrollDistanceX) {
			transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera> ().orthographicSize * (16.0 / 9.0) - scrollDistanceX), defaultY, transform.position.z);
		}
		if (player.transform.position.y > transform.position.y + (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
			transform.position = new Vector3 (transform.position.x, defaultY, player.transform.position.z - (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY));
		}
		else if (player.transform.position.y < transform.position.y - (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
			transform.position = new Vector3 (transform.position.x, defaultY, player.transform.position.z + (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY));
		}
	}

	public void Reset() {
		transform.position = startingPos;
	}
}
