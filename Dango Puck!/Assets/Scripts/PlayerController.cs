using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary { public float zMin, zMax; }﻿

public class PlayerController : MonoBehaviour {
	//get game controller
	private GameController gameController;

	private Rigidbody rb;
	//private AudioSource aud;
	public float speed;
	public float movementFrameSpeed;
	public GameObject playerLine;

	//keyCode for player control
	public string upButtonString;
	public string downButtonString;
	private KeyCode upKeyCode;
	private KeyCode downKeyCode;

	//public float tilt; 
	public Boundary boundary;

	void Start() {
		//get game controller
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}

		rb = GetComponent<Rigidbody> ();
		//aud = GetComponent<AudioSource> ();

		upKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), upButtonString) ;
		downKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), downButtonString) ;
		if (upButtonString == "" || downButtonString == "") {
			upKeyCode = KeyCode.B;
			downKeyCode = KeyCode.B;
		}
	}

	void Update() {
		//constraint. this object should not have any forces on it
		rb.velocity = Vector3.zero;
	}

	void FixedUpdate() {
		//movement control
		float moveUp = 0.0f;
		bool upPressed = Input.GetKey (upKeyCode);
		bool downPressed = Input.GetKey (downKeyCode);
		if (upPressed && !downPressed) {
			moveUp = movementFrameSpeed;
		} else if (!upPressed && downPressed) {
			moveUp = -1 * movementFrameSpeed;
		}
		float movementUp = gameController.getCanPlayersCanMove() ? moveUp * speed : 0.0f;

		//bound detection
		Vector3 bounds = new Vector3 (rb.position.x, rb.position.y, (Mathf.Clamp(rb.position.z + movementUp, boundary.zMin, boundary.zMax))); 
		rb.position = bounds; 

		//constraint. this object should not have any forces on it
		rb.velocity = Vector3.zero;
	}
}

