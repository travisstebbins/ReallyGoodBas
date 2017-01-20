using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// public variables
	public float scrollDistanceXProportion = 0.7f;
	public float scrollDistanceYProportion = 0.7f;
	public float defaultZ = -10f;

	// private variables
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
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, defaultZ);
	}

	void Update () {
		if (player.transform.position.x > transform.position.x + (this.GetComponent<Camera>().orthographicSize * (16.0/9.0) - scrollDistanceX)) {
			transform.position = new Vector3 (player.transform.position.x - (float)(this.GetComponent<Camera> ().orthographicSize * (16.0 / 9.0) - scrollDistanceX), transform.position.y, defaultZ);
		}
		else if (player.transform.position.x < transform.position.x - (this.GetComponent<Camera>().orthographicSize * (16.0/9.0)) + scrollDistanceX) {
			transform.position = new Vector3 (player.transform.position.x + (float)(this.GetComponent<Camera> ().orthographicSize * (16.0 / 9.0) - scrollDistanceX), transform.position.y, defaultZ);
		}
		if (player.transform.position.y > transform.position.y + (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
			transform.position = new Vector3 (transform.position.x, player.transform.position.y - (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY), defaultZ);
		}
		else if (player.transform.position.y < transform.position.y - (this.GetComponent<Camera>().orthographicSize - scrollDistanceY)) {
			transform.position = new Vector3 (transform.position.x, player.transform.position.y + (float)(this.GetComponent<Camera>().orthographicSize - scrollDistanceY), defaultZ);
		}
	}

	public void Reset() {
		transform.position = startingPos;
	}
}
