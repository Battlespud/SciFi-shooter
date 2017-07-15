using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator : MonoBehaviour {

	public float TopStop;
	public float BottomStop;
	public float speed;

	float modifier = 1f;

	Vector3 moveVector;

	CharacterController cc;

	// Use this for initialization
	void Start () {
		moveVector = new Vector3 (0f, speed, 0f);
		cc = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y > TopStop || transform.position.y < BottomStop) {
			turnaround ();
		}
		cc.Move ((moveVector * modifier*Time.deltaTime));
	}

	void turnaround(){
		modifier *= -1f;
	}
}
