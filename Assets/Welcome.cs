using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Welcome : MonoBehaviour {

	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log ("Player has joined!");
	}
}
