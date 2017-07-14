using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IActivate {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Activate(){
		Debug.Log ("tried to open the door lul");
	}

	public string GetToolTip(){
		return "Press F to Open Door";
	}
}
