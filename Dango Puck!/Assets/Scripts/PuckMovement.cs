using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckMovement : MonoBehaviour {
	private GameController gameController;

	private Rigidbody rb;
	public float initialSpeed;
	public float speedMultiplier;
	public int maxTimesSpeedIncrease;
	private Vector3 initialDirection;
	private Vector3 oldVelocity;
	private int timesSpeedIncreased;
	private string recentPlayerTag;

	public bool isRandomInitialVelocity;
	public float rotationSpeed;
	public float maxAngleShootOffFromHorizontal;
	public float minAngleFromAxis;

	//animation vars for state
	private int animationState = 0;

	private bool isMoving;

	//audio
	private AudioSource[] audioSource;
	private AudioSource highContactSound;
	private AudioSource lowContactSound;

	//>< dango puck face 
	public Material dangoPainFaceImage;
	private Renderer rend;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		timesSpeedIncreased = 0;
		recentPlayerTag = "any";

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
		//material
		rend = GetComponent<Renderer> ();

		//audio
		audioSource = GetComponents<AudioSource> ();
		highContactSound = audioSource[0];
		lowContactSound = audioSource[1];

		isMoving = false;
		//ShootOff ();

	}
	bool isWithinAngleFromXAxis(Vector2 direction, float angleFromXAxis) {
		float angle = Vector2.Angle( direction, new Vector2(1.0f,0.0f) );
		Debug.Log ("ANGLE");
		Debug.Log (angle);

		//check if the angle is within the angle range specified
		if ((angle <= angleFromXAxis) || (angle >= (180f-angleFromXAxis))) {
			return true;
		}
		//Debug.Log ("NOT WITHIN RANGE OF ANGLE");
		return false;
	}
	bool isOutsideAngleFromXAxis(Vector2 direction, float angleFromXAxis) {
		float angle = Vector2.Angle( direction, new Vector2(1.0f,0.0f) );
		//check if the angle is within the angle range specified
		if ((angle <= angleFromXAxis) || (angle >= (180f-angleFromXAxis))) {
			//Debug.Log ("NOT OUTSIDE RANGE OF ANGLE");
			return false;
		}
		return true;
	}
	bool isOutsideAngleFromYAxis(Vector2 direction, float angleFromYAxis) {
		float angle = Vector2.Angle( direction, new Vector2(0.0f,1.0f) );
		//check if the angle is within the angle range specified
		if ((angle <= angleFromYAxis) || (angle >= (180f-angleFromYAxis))) {
			//Debug.Log ("NOT OUTSIDE RANGE OF ANGLE");
			return false;
		}
		return true;
	}
	void ShootOff() {
		if (isRandomInitialVelocity) {
			//initial random direction
			Vector2 randomUnitDirection = Random.insideUnitCircle.normalized;

			//loop until we get a direction that gives us a reasonable first shot
			while (!isWithinAngleFromXAxis(randomUnitDirection, maxAngleShootOffFromHorizontal) || !isOutsideAngleFromXAxis(randomUnitDirection, minAngleFromAxis)) {
				randomUnitDirection = Random.insideUnitCircle.normalized;
			}
			initialDirection = new Vector3(randomUnitDirection.x, 0.0f, randomUnitDirection.y);
		} else {
			if (Random.value > 0.5) {
				initialDirection = new Vector3(-1.0f, 0.0f, 0.0f);
			} else {
				initialDirection = new Vector3(1.0f, 0.0f, 0.0f);
			}
		}

		rb.velocity = initialDirection * initialSpeed;
		Debug.Log (rb.velocity);
	}

	void PlayAnimation() {
		float newRotationSpeed = (rotationSpeed + ((rotationSpeed / 1.5f) * gameController.getTimesPuckRespawned ()));
		transform.Rotate (new Vector3 (0.0f, 1.0f, 0.0f) * newRotationSpeed * Time.deltaTime);
	}
	
	// Update is called once per frame
	void Update () {
		//check game controller's state variable.
		string state = gameController.getGameState ();
		if (state == "NEWPUCKANIMATION") {
			PlayAnimation ();
		} else if (state == "PUCKINPLAY" && isMoving == false) {
			isMoving = true;
			ShootOff ();

		}
	}

	void FixedUpdate() {
		oldVelocity = rb.velocity;
		//Debug.Log (rb.velocity);
		Debug.Log (rb.velocity.magnitude);
	}


	//collision
	void OnCollisionEnter(Collision collision) {
		ContactPoint contact = collision.contacts [0];
		Debug.Log ("collision tag");
		Debug.Log (collision.collider.tag);//CHECK COLLISION TAG . DEBUG

		Debug.Log ("in velocity");
		Debug.Log (oldVelocity);

		bool playHighSound = false;

		if (collision.collider.tag == "West Player" || collision.collider.tag == "East Player") {
			playHighSound = true;
		}
		if (collision.collider.tag != "East Goal" && collision.collider.tag != "West Goal") {
			ReflectPuck (collision.collider.tag, contact.normal, playHighSound);
		}
		Debug.Log ("out velocity");
		Debug.Log (rb.velocity);
	}

	void ReflectPuck(string colliderTag, Vector3 normalVector, bool playHighSound) {
		Debug.Log ("HEY");
		Debug.Log (colliderTag);
		Debug.Log (recentPlayerTag);
		bool timesSpeedIncreasedNotMaxed = (timesSpeedIncreased < maxTimesSpeedIncrease);
		/*if ((timesSpeedIncreasedNotMaxed && recentPlayerTag == "any") || (timesSpeedIncreasedNotMaxed && recentPlayerTag=="West Player" && colliderTag=="East Player") || (timesSpeedIncreasedNotMaxed && recentPlayerTag=="East Player" && colliderTag=="West Player")  ) {
			rb.velocity = (Vector3.Reflect (oldVelocity, normalVector)) * speedMultiplier;
			timesSpeedIncreased++;
			recentPlayerTag = colliderTag;
		} */
		//reflect the vector
		Vector3 reflectedVector = Vector3.Reflect (oldVelocity, normalVector);
		//we do not want the puck to ever go directly up/down or side-to-side cause nobody would be able to hit the puck then
		if (reflectedVector.x == 0.0f) {
			reflectedVector.Set (0.25f, reflectedVector.y, reflectedVector.z);
		} else if (reflectedVector.z == 0.0f) {
			reflectedVector.Set (reflectedVector.x, reflectedVector.y, 0.25f);
		}

		if (timesSpeedIncreasedNotMaxed && (colliderTag == "West Player" || colliderTag == "East Player")) {
			rb.velocity = reflectedVector * speedMultiplier;
			timesSpeedIncreased++;
			recentPlayerTag = colliderTag;
			if (timesSpeedIncreased > 4) {
				rend.material = dangoPainFaceImage;
			}
		} else {//max times speed increased. will maintain this speed for rest of game
			rb.velocity = reflectedVector;

		}
		if (playHighSound) {
			highContactSound.Play ();
		} else {
			lowContactSound.Play ();
		}
	}
}
