using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetector : MonoBehaviour {

	private GameController gameController;
	//public GameObject newPuckObject;


	// Use this for initialization
	void Start () {

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	// Update is called once per frame
	void Update () {

	}

	// Goal detector
	void OnTriggerExit(Collider other) {
		//increment the score by telling if this is west score or east score. 
		//tags are assigned to the goal detectors
		gameController.Scored (this.tag);
	}
}
