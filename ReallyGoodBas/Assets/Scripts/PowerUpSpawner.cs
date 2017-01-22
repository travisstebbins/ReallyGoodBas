using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour {

	// public variables
	public int minRespawnTime = 30;
	public int maxRespawnTime = 60;
	public GameObject powerUpPrefab;

	// components
	private GameObject item;

	void Start () {
		StartCoroutine (SpawnItemCoroutine (minRespawnTime, maxRespawnTime));
	}

	void SpawnItem () {
		item = (GameObject) Instantiate (powerUpPrefab, transform.position, Quaternion.Euler(90, 0, 0));
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Player")) {
			Debug.Log ("item collision");
			Destroy (item.gameObject);
			StartCoroutine (SpawnItemCoroutine (minRespawnTime, maxRespawnTime));
		}
	}

	IEnumerator SpawnItemCoroutine (int minRespawnTime, int maxRespawnTime) {
		int spawnTime = UnityEngine.Random.Range (minRespawnTime, maxRespawnTime + 1);
		yield return new WaitForSeconds (spawnTime);
		SpawnItem ();
	}
}
