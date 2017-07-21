using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	void OnCollisionEnter(Collision col){
		if(col.collider.gameObject.GetComponent<Player>()!=null){
			col.collider.gameObject.GetComponent<Player>().Damage(3);
		}
		else
		if(col.collider.gameObject.GetComponentInParent<Player>()!=null){
			col.collider.gameObject.GetComponentInParent<Player>().Damage(3);
		}
		else
		if(col.collider.gameObject.GetComponentInChildren<Player>()!=null){
			col.collider.gameObject.GetComponentInChildren<Player>().Damage(3);
		}
		if(col.gameObject.CompareTag("Reflective")){
			return;
		}
		Debug.Log (col.gameObject.name);
		Destroy(gameObject);

	}




}
