using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowSpawn : MonoBehaviour {
	public int snowCount;
	public float xMax;
	public float yMax;
	public float startYPosition;
	public GameObject snowObject;
	public float spawnWait;
	public float waveWait;
	public GameObject canvas;

	// Use this for initialization
	void Start () {

		StartCoroutine (SpawnSnow ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SpawnSnow() {
		while (true) {
			for (int i = 0; i < snowCount; i++) {
				Vector3 spawnPosition = new Vector3 (Random.Range (0, xMax), startYPosition, 0.0f);
				Quaternion spawnRotation = Quaternion.identity;
				//Instantiate (snowObject, spawnPosition, spawnRotation);
				Instantiate (snowObject, spawnPosition, spawnRotation, canvas.transform);
				//snowObject.transform.parent = canvas.transform;
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
		yield return null;
	}
}
