using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFalling : MonoBehaviour {
	private RectTransform rectTrans;
	public float fallingSpeed;
	public float lowYBound;

	// Use this for initialization
	void Start () {
		rectTrans = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		float goTextPosY = rectTrans.anchoredPosition.y;
		float goTextPosX = rectTrans.anchoredPosition.x;

		if (goTextPosY < lowYBound) {
			Destroy (this);
		} else {
			rectTrans.anchoredPosition = new Vector2 (goTextPosX, goTextPosY - fallingSpeed);
		}
	}
}
