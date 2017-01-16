using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNavigation : MonoBehaviour {
	private AudioSource[] audioSources;
	private AudioSource buttonSound;


	// Use this for initialization
	void Start () {
		audioSources = GetComponents<AudioSource> ();
		Debug.Log (audioSources.Length);
		if (audioSources.Length > 1) {
			buttonSound = audioSources [1];
		} else {
			buttonSound = null;
			Debug.Log ("button sound is not attached");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey && Application.loadedLevelName == "HowToPlay") {
			GoToMainMenu ();
		}
	}

	public void GoToGameplay() {
		//SceneManager.LoadScene ("MainGameplay");

		Application.LoadLevel ("MainGameplay");
		buttonSound.Play ();
	}

	public void GoToHowToPlay() {
		buttonSound.Play ();
		Application.LoadLevel ("HowToPlay");

	}

	public void GoToMainMenu() {
		
		Application.LoadLevel ("MainMenu");
		buttonSound.Play ();
	}
}
/*
   var SkateboardRoll: AudioSource; 
 var SkateboardLand: AudioSource;
 var SkateboardJump: AudioSource;
 
 function Start(){
     var aSources = GetComponents(AudioSource);
     SkateboardRoll = aSources[0];
     SkateboardLand = aSources[1];
     SkateboardJump = aSources[2];
 
     SkateboardRoll.Play();
 
     yield WaitForSeconds (1);
 
     SkateboardLand.Play();
 
     yield WaitForSeconds (1);
 
     SkateboardJump.Play();
 }
 */
