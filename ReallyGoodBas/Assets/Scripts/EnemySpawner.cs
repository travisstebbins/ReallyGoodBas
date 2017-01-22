using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Diagnostics;

public class EnemySpawner : MonoBehaviour {

	// public variables
	public int minSpawnRate = 15;
	public int maxSpawnRate = 20;
	public GameObject redEnemy;
	public GameObject greenEnemy;
	public GameObject blueEnemy;

	// private variables
	private PlayerController player;
	private int spawnCount = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		StartCoroutine (SpawnEnemyCoroutine (minSpawnRate, maxSpawnRate));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SpawnEnemyCoroutine (int minSpawnRate, int maxSpawnRate) {
		while (true) {
			int spawnTime = UnityEngine.Random.Range (minSpawnRate, maxSpawnRate + 1);
			yield return new WaitForSeconds (spawnTime);
			int rand = UnityEngine.Random.Range (0, 3);
			GameObject spawnedEnemy;
			switch (rand) {
			case 0:
				spawnedEnemy = Instantiate (redEnemy, transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
				spawnedEnemy.GetComponent<EnemyController> ().setColor (0);
				player.AddColoredObject (0, spawnedEnemy);
				break;
			case 1:
				spawnedEnemy = Instantiate (greenEnemy, transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
				spawnedEnemy.GetComponent<EnemyController> ().setColor (1);
				player.AddColoredObject (1, spawnedEnemy);
				break;
			case 2:
				spawnedEnemy = Instantiate (blueEnemy, transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
				spawnedEnemy.GetComponent<EnemyController> ().setColor (2);
				player.AddColoredObject(2, spawnedEnemy);
				break;
			}
			spawnCount++;
			if (spawnCount % 5 == 0 && minSpawnRate > 1) {
				minSpawnRate--;
				maxSpawnRate--;
			}
		}
	}
}
