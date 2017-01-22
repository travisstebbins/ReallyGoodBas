using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	// private variables
	private Text timerText;
	private float timeElapsed = 0f;

	void Start () {
		timerText = GetComponent<Text> ();
	}

	void Update () {
		timeElapsed += Time.deltaTime;
		timerText.text = Mathf.RoundToInt (timeElapsed).ToString ();
	}
}