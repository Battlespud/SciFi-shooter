using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;





public enum MoveSpeed{
	WALK = 800,
	RUN = 1400,
	SPRINT = 2000
};


public class MovementController : NetworkBehaviour {

	public float sensitivity = 2f;
	public Camera cam;

	Rigidbody rb;
	MoveSpeed moveSpeed;
	public Vector3 toMove;
	bool toJump = false;
	public static Vector3 JumpForce = new Vector3 (0f, 300f, 0f);



	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			GrabReferences ();
			moveSpeed = MoveSpeed.RUN;
			toggleLock ();
		} else {
			cam.enabled = false;
		}
	}

	void GrabReferences(){
		rb = GetComponent<Rigidbody> ();
	}

	public override void OnStartLocalPlayer(){
		GetComponent<MeshRenderer>().material.color = Color.green;

	}
	
	// Update is called once per frame
	void Update () {
		toMove = new Vector3 ();
		if (isLocalPlayer) {
			CatchInput ();
			Move ();
		}
	}

	void CatchInput(){
		if (Input.GetKey (KeyBinds.Forward))
			toMove += transform.forward;
		if (Input.GetKey (KeyBinds.Back))
			toMove += transform.forward*-1;
		if (Input.GetKey (KeyBinds.Left))
			toMove += transform.right*-1;
		if (Input.GetKey (KeyBinds.Right))
			toMove += transform.right;
		if (Input.GetKeyDown (KeyBinds.Jump))
			toJump = true;
		if (Input.GetAxis ("Mouse X") < 0)
			transform.Rotate (Vector3.up * -sensitivity);
		if (Input.GetAxis ("Mouse X") > 0)
			transform.Rotate (Vector3.up * sensitivity);
		if (Input.GetKeyDown (KeyCode.G)) 
			toggleLock ();
	}


	void toggleLock(){
		if (Cursor.visible) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

			void Move(){
		rb.AddForce (toMove.normalized * (float)moveSpeed * Time.deltaTime);
		if (toJump) {
			rb.AddForce (JumpForce);
			toJump = false;
		}
	}



}
