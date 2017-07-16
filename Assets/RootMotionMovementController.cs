using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionMovementController : MonoBehaviour {


	public Weapon weapon;

	float inertia = .8f;

	Animator ani;

	bool isCrouching = false;

	Vector3 toMove;

	float speed = 1f;

	bool PistolOut=false;


	// Use this for initialization
	void Start () {
		ani = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();
		UpdateAnimations ();
	}


	void GetInput(){
		float x = 0f;
		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = Mathf.Lerp (speed, 3f, 1f * Time.deltaTime);
		} else {
			speed = Mathf.Lerp (speed, 1f, 1f * Time.deltaTime);
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			ani.SetTrigger ("Jump");
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			isCrouching = !isCrouching;
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			ani.SetTrigger ("Slide");
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			PistolOut = !PistolOut;
		}

		if (Input.GetKey (KeyCode.Mouse0)) {
			weapon.Fire ();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			weapon.Reload();
		}

		if( Input.GetAxis ("Mouse X") > 0){
			x = 1f;
		}
		else if( Input.GetAxis ("Mouse X") < 0){
			x = -1f;
		}
		ani.SetFloat ("Turn", x);

	//	ani.SetFloat ("Turn", Input.GetAxis ("Mouse X"));

		ani.SetFloat("Speed", speed);
		//TODO FIX BACKWARDS MOVEMENT
		if (Input.GetAxis ("Vertical") != 0) {
			if (Mathf.Abs(toMove.z) >= 1f) {
				toMove.z = Mathf.Lerp (toMove.z, speed*Input.GetAxis("Vertical"), 1f * Time.deltaTime);
			} else {
				if (Input.GetAxis ("Vertical") > 0) {
					toMove.z = 1f;
				}
				else{
					toMove.z = -1f;
				}
			}
		}
		else{
			toMove.z = Mathf.Lerp (toMove.z, 0f, inertia * Time.deltaTime);
			if (Mathf.Abs(toMove.z) < .2f)
				toMove.z = 0f;
		}

		if (Input.GetAxis ("Horizontal") != 0) {
			if (Mathf.Abs(toMove.x) >= 1f) {
				toMove.x = Mathf.Lerp (toMove.x, speed*Input.GetAxis("Horizontal"), 1f * Time.deltaTime);
			} else {
				if (Input.GetAxis ("Horizontal") > 0) {
					toMove.x = 1f;
				}
				else{
					toMove.x = -1f;
				}
			}
		}
		else{
			toMove.x = Mathf.Lerp (toMove.x, 0f, inertia * Time.deltaTime);
			if (Mathf.Abs(toMove.x) < .2f)
				toMove.x = 0f;
		}

	}

	void UpdateAnimations(){
		ani.SetBool ("isCrouching", isCrouching);
		ani.SetFloat ("VelocityRight", toMove.x);
		ani.SetFloat ("VelocityForward", toMove.z);
		ani.SetBool ("PistolOut", PistolOut);
	}
}
