using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GameController : MonoBehaviour {
	private int westScore;
	private int eastScore;
	public GUIText westScoreText;
	public GUIText eastScoreText;

	//player disabled state
	private bool playersAbleToMove;

	//go text
	public Sprite brightGoTextImage; //Drag your second sprite here in inspector.
	//game over text
	public Sprite eastWinImage;
	public Sprite westWinImage;
	private GameObject winImageObject;

	// game over text
	public GUIText gameOverText;
	public GameObject newPuckObject;//puck with normal eyes
	public GameObject newPuck2Object;//puck with >< eyes
	private int timesPuckRespawned;

	bool restartEnabled;

	// get positions for the lamps
	private Vector3 westLamp1Position;
	private Vector3 westLamp2Position;
	private Vector3 westLamp3Position;
	private Vector3 eastLamp3Position;
	private Vector3 eastLamp2Position;
	private Vector3 eastLamp1Position;

	// lamp ON objects
	public GameObject newWestLampOn;
	public GameObject newEastLampOn;

	//audio
	private AudioSource[] audioSource;
	private AudioSource defaultSong;
	private AudioSource funkSong;
	private AudioSource swingSong;
	private AudioSource highBlopSound;
	private AudioSource lowBlopSound;
	private AudioSource dingSound;
	private AudioSource victorySound;
	private AudioSource whistleSound;
	private AudioSource goSound;

	//Time and position vars for beginning gameplay animations
	public float showStageSeconds;
	//GO text variables
	public float goTextSpeed;
	public float goTextStartPositionY;
	public float goTextEndPositionY;
	private GameObject goTextObject;
	private RectTransform goTextTransform;
	public float goTextFreezeTimeSeconds;
	public float betweenScoreSpawnTimeSeconds;
	public float newPuckAnimationSeconds;
	public float betweenGameOverWinnerTimeSeconds;

	//variables for start text and new round/new puck
	//states for game
	//CONSTS: { SHOWSTAGE, TEXTMOVING, TEXTFREEZE(call coroutine with parameter time), NEWPUCKANIMATION, PUCKSTART, PUCKINPLAY
	private string gameState;



	// Use this for initialization
	void Start () {
		restartEnabled = false;

		westScore = 0;
		eastScore = 0;
		westScoreText.text = westScore + "";
		eastScoreText.text = eastScore + "";

		playersAbleToMove = false;

		GameObject puckObject = newPuckObject;
		if (puckObject != null) {
			//
		}
		if (puckObject == null) {
			Debug.Log ("Cannot find Puck object");
		}
		timesPuckRespawned = 0;

		gameOverText.text = "";

		//save positions of all lamps. hope for the best that they are all found.
		westLamp1Position = GameObject.FindWithTag ("West Lamp 1").transform.position;
		westLamp2Position = GameObject.FindWithTag ("West Lamp 2").transform.position;
		westLamp3Position = GameObject.FindWithTag ("West Lamp 3").transform.position;
		eastLamp1Position = GameObject.FindWithTag ("East Lamp 1").transform.position;
		eastLamp2Position = GameObject.FindWithTag ("East Lamp 2").transform.position;
		eastLamp3Position = GameObject.FindWithTag ("East Lamp 3").transform.position;

		//go text vars
		goTextObject = GameObject.FindWithTag ("GO Text");
		goTextTransform = goTextObject.GetComponent<RectTransform> ();
		Debug.Log (goTextTransform);

		//win text vars
		winImageObject = GameObject.FindWithTag("Win Image");
		winImageObject.SetActive (false);

		//audio
		audioSource = GetComponents<AudioSource> ();
		defaultSong = audioSource[0];
		funkSong = audioSource[1];
		swingSong = audioSource[2];
		highBlopSound = audioSource[3];
		lowBlopSound = audioSource[4];
		dingSound = audioSource[5];
		victorySound = audioSource[6];
		whistleSound = audioSource [7];
		goSound = audioSource [8];


		//GAME STATE
		gameState = "SHOWSTAGE";

		StartCoroutine (GamePlay());
	}
	
	// Update is called once per frame
	void Update () {
		if (restartEnabled) {
			if (Input.GetKeyDown (KeyCode.R)) {
				Application.LoadLevel (Application.loadedLevel);
			} else if (Input.GetKeyDown (KeyCode.M)) {
				Application.LoadLevel ("MainMenu");
			}
		}
		Debug.Log (gameState);
	}

	// Game Over
	void GameOver() {
		gameState = "GAMEOVER";
		Image winImageObjectImageComponent = winImageObject.GetComponent<Image> ();
		if (westScore >= 3) {
			winImageObjectImageComponent.sprite = westWinImage;
		} else {
			winImageObjectImageComponent.sprite = eastWinImage;
		}
		winImageObject.SetActive (true);
		gameOverText.text = "Press 'r' to restart or 'm' for main menu";
		restartEnabled = true;
		swingSong.Stop ();
		victorySound.Play ();
		playersAbleToMove = false;
	}

	// New Round
	void NewRound() {
		Vector3 spawnPosition = new Vector3 (0.0f, 0.0f, 0.0f);
		Quaternion spawnRotation = Quaternion.identity;
		spawnRotation.Set (0.0f, 180.0f, 0.0f, 0.0f);
		Instantiate(newPuckObject,spawnPosition,spawnRotation);
		timesPuckRespawned++;
	}

	// Increment Score
	public void Scored (string tag) {
		
		GameObject puckObject = GameObject.FindWithTag ("Puck");
		Destroy (puckObject);

		GameObject lampObject;
		if (tag == "West Goal") {
			eastScore++;
			//eastScoreText.text = eastScore + "";
			string lampTagToFind = "East Lamp " + eastScore;
			lampObject = GameObject.FindWithTag (lampTagToFind);
			if (eastScore == 1) {
				Instantiate (newEastLampOn, eastLamp1Position, Quaternion.identity);
			} else if (eastScore == 2) {
				Instantiate (newEastLampOn, eastLamp2Position, Quaternion.identity);
			} else {
				Instantiate (newEastLampOn, eastLamp3Position, Quaternion.identity);
			}
			//destroy the OFF lamp that we just turned on.
			Destroy(lampObject);
				
		} else if (tag == "East Goal") {
			westScore++;
			//westScoreText.text = westScore + "";
			string lampTagToFind = "West Lamp " + westScore;
			lampObject = GameObject.FindWithTag (lampTagToFind);
			if (westScore == 1) {
				Instantiate (newWestLampOn, westLamp1Position, Quaternion.identity);
			} else if (westScore == 2) {
				Instantiate (newWestLampOn, westLamp2Position, Quaternion.identity);
			} else {
				Instantiate (newWestLampOn, westLamp3Position, Quaternion.identity);
			}
			//destroy the OFF lamp that we just turned on.
			Destroy(lampObject);
		}
		//play ding sound for score
		dingSound.Play ();

		//check if game over
		if (westScore >= 3 || eastScore >= 3) {
			StartCoroutine (DelayedGameOver ());

		} else {
			gameState = "NEWPUCKANIMATION";
			StartCoroutine (GamePlay());
			//NewRound ();
		}
			
	}

	public bool getCanPlayersCanMove() {
		return playersAbleToMove;
	}

	public string getGameState() {
		return gameState;
	}
	public void setGameState(string newState) {
		gameState = newState;
	}

	public int getTimesPuckRespawned() {
		return timesPuckRespawned;
	}

	IEnumerator HoldGameStateThenChange(int haltSeconds, string nextState) {
		yield return new WaitForSeconds (haltSeconds);
		gameState = nextState;
	}

	IEnumerator GamePlay() {
		while (true) {
			Debug.Log ("in ienumerator");
			if (gameState == "SHOWSTAGE") {
				yield return new WaitForSeconds (showStageSeconds);
				//Debug.Log ("waited done");
				gameState = "TEXTMOVING";
				//Debug.Log (gameState);
			} else if (gameState == "TEXTMOVING") {
				float goTextPosY = goTextTransform.anchoredPosition.y;
				float goTextPosX = goTextTransform.anchoredPosition.x;
				Debug.Log (goTextTransform.anchoredPosition.y);
				if (goTextPosY <= goTextEndPositionY) {
					gameState = "TEXTFREEZE";
					goTextObject.GetComponent<Image> ().sprite = brightGoTextImage;

				} else {//keep the text moving
					goTextTransform.anchoredPosition = new Vector2 (goTextPosX, goTextPosY - goTextSpeed);
				}

			} else if (gameState == "TEXTFREEZE") {
				goSound.Play ();
				yield return new WaitForSeconds (goTextFreezeTimeSeconds);
				gameState = "NEWPUCKANIMATION";
				Destroy (goTextObject);
			} else if (gameState == "NEWPUCKANIMATION") {
				yield return new WaitForSeconds (betweenScoreSpawnTimeSeconds);
				playersAbleToMove = true;
				NewRound ();

				whistleSound.Play ();
				yield return new WaitForSeconds (newPuckAnimationSeconds);
				gameState = "PUCKINPLAY";
			} else if (gameState == "PUCKINPLAY") {
				break;//change state here? da fuk do i do here
				//if detect a puck score, then change state to newpuckanimation
			} else if (gameState == "GAMEOVER") {
				playersAbleToMove = false;
			}
			yield return null;
		}
		yield return null;
	}

	//Delayed game over function waits a few seconds before deciding the game is over
	IEnumerator DelayedGameOver() {
		yield return new WaitForSeconds (betweenGameOverWinnerTimeSeconds);
		GameOver ();
	}

}
