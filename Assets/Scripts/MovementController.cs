using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;





public enum MoveSpeed{
	AIM = 4,
	WALK = 6,
	RUN = 12,
};


public class MovementController : NetworkBehaviour {

	Player player;
	Weapon weapon;

	const float GRAVITY = -18.8f; //acceleration
	public float xSensitivity = 1f;
	public float ySensitivity = 1f;
	public Camera cam;

	public Texture2D cursor;

	float yaw = 0f;
	float pitch = 0f;

	CharacterController controller;
	MoveSpeed moveSpeed;
	public Vector3 toMove;
	bool toJump = false;
	public int jumpCounter = 0;
	public int jumpLimit = 1;

	bool WeaponDrawn = false;
	bool lockoutDraw = false;
	bool AimingSights =false;
	bool lockoutAim = false;

	bool lockout = false;

	//cam
	Vector3 camDefaultPosition;
	Vector3 camOverShoulderPosition= new Vector3(1f, .5f, -2.5f);

	float defaultFov = 90f;
	float sightsFov = 45f;

	//Physics Emulation
	float mass = 5f;
	float jumpForce = 5f;
	Vector3 impact;	


	//Gravity
	public bool usesGravity = true;
	float gravityCheckOffsetDistance =.2f;
	float gravityCheckDistance;
	public bool gravityEnabled;
	public float currGravity;
	float gravityTimer;
	float mGravityTimer;
	float Interval = 20f;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			player = GetComponent<Player> ();
			Cursor.SetCursor (cursor, new Vector2(16,16), CursorMode.ForceSoftware);
			GrabReferences ();
			moveSpeed = MoveSpeed.WALK;
			toggleLock ();
			GravitySetup ();
		} else {
			cam.enabled = false;
			GetComponent<AudioListener> ().enabled = false;
		}
	}

	void GrabReferences(){
		controller = GetComponent<CharacterController> ();
		camDefaultPosition = cam.transform.localPosition;
		pitch = cam.transform.rotation.eulerAngles.x;
		yaw = transform.rotation.eulerAngles.y;
		weapon = GetComponentInChildren<Weapon> ();
	}

	public override void OnStartLocalPlayer(){
		GetComponent<MeshRenderer>().material.color = Color.green;

	}
	
	// Update is called once per frame
	void Update () {
		toMove = new Vector3 ();
		if (isLocalPlayer) {
			Collisions ();
			GravityLoop ();
			if (!lockout) {
				CatchInput ();
			}
			Move ();
		}
	}

	public void CmdDamage(int i, Player p){
		p.DoDamageTest (i);
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
			Jump ();
		if (Input.GetKeyDown (KeyCode.G)) 
			toggleLock ();
		if (Input.GetKeyDown (KeyCode.Escape)) 
			Unlock ();
		if (Input.GetKeyDown (KeyCode.R)) 
			
			weapon.Reload ();
		if (Input.GetMouseButtonDown (0))
		if (WeaponDrawn) {
			weapon.Fire ();
		} else {
			ToggleHolster ();
		}
		if (Input.GetMouseButtonDown(1))
			ToggleSights();
		if (Input.GetKey (KeyCode.LeftShift)) {
			if (AimingSights)
				ToggleSights ();
			if (!AimingSights && (player.Stamina > 2 || (player.Stamina > 0 && player.usingStamina))) {
				moveSpeed = MoveSpeed.RUN;
				player.usingStamina = true;
				player.Stamina -= Time.deltaTime;
			} else {
				player.usingStamina = false;
				if (AimingSights) 	moveSpeed = MoveSpeed.AIM;
				else{	moveSpeed = MoveSpeed.WALK;	}
			}
		}else {
			player.usingStamina = false;
			if (AimingSights) 	moveSpeed = MoveSpeed.AIM;
			else{	moveSpeed = MoveSpeed.WALK;	}
		}
		if (Input.GetKeyDown(KeyBinds.ToggleWeapon))
			ToggleHolster();
	}


	void ToggleSights(){
		if (!AimingSights && !lockoutAim && (WeaponDrawn || !lockoutDraw)) {
			AimingSights = true;
			if (!WeaponDrawn) {
				ToggleHolster ();
			}
			lockoutAim = true;
			StartCoroutine(ShiftFov(sightsFov,.5f));
		} else if(!lockoutAim) {
			AimingSights = false;
			lockoutAim = true;
			StartCoroutine(ShiftFov(defaultFov,.5f));
		}
	}

	private IEnumerator ShiftFov(float finish, float time){
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,finish,(elapsedTime/time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		lockoutAim = false;
	}

	void ToggleHolster(){
		if (WeaponDrawn && !lockoutDraw) {
			WeaponDrawn = false;
			toggleLock ();
			lockoutDraw = true;
			StartCoroutine (ShiftPosition (camDefaultPosition, .5f));
		} else if(!lockoutDraw) {
			WeaponDrawn = true;
			toggleLock ();
			lockoutDraw = true;
			StartCoroutine (ShiftPosition (camOverShoulderPosition, .5f));
		}
	}

	private IEnumerator ShiftPosition(Vector3 finish, float time){
		float elapsedTime = 0f;
		while (elapsedTime < time) {
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition,finish,(elapsedTime/time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		lockoutDraw = false;
	}



	void toggleLock(){
		if (Cursor.visible) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	void Unlock(){
		if (Cursor.lockState == CursorLockMode.None) {
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	void Move(){
		controller.Move (toMove.normalized * (float)moveSpeed * Time.deltaTime);
		if (toJump) {
			AddImpact (transform.up, jumpForce);
			toJump = false;
		}
	}

	void ForceMove(Vector3 vec){
		controller.Move (vec * Time.deltaTime);
	}

	void ForceMoveBypass(Vector3 vec){
		controller.Move (vec * Time.deltaTime);
	}

	void Jump(){
		if (IsGrounded()) {
			toJump = true;
		} else if (jumpCounter < jumpLimit) {
			toJump = true;
			jumpCounter++;
		}

	}

	public bool IsGrounded(){
		if (!gravityEnabled) {
			return true;
		} else
			return false;
	}

	void Collisions(){
		if (impact.magnitude > .1f) {
			controller.Move ( (impact));
		//	lockout = true;
			impact = Vector3.Lerp (impact, Vector3.zero, 5 * Time.deltaTime);
		} else {
		//	lockout = false;
		}
	}

	public void AddImpact(Vector3 dir, float force){
		dir.Normalize ();
		if (dir.y < 0)
			dir.y = -dir.y;
		impact += dir.normalized * force / mass;
	}

	void GravitySetup(){
			mGravityTimer = 1 / Interval;
			gravityCheckDistance = GetComponent<Collider> ().bounds.size.y / 2 + gravityCheckOffsetDistance;
	}

	private void GravityLoop(){
		if (usesGravity) {
			gravityTimer += Time.fixedUnscaledDeltaTime;
			if (gravityTimer >= mGravityTimer) {
				gravityTimer -= mGravityTimer;
				CheckGravity ();
			}
			if (gravityEnabled) {
				currGravity += GRAVITY * Time.deltaTime;
				ForceMoveBypass (new Vector3 (0f, currGravity, 0f));
			} else {
				currGravity = 0f; //reset to base acceleration
				jumpCounter = 0;
			}
		} else {
			gravityEnabled = false; //in case we change mid game for some reason
		}
	}

	private void CheckGravity(){ //TODO literally just doesnt work idfk why
		RaycastHit hit;
		//Debug.DrawRay (transform.position, Vector3.down * gravityCheckDistance); 
		Ray GravityCheckingRay = new Ray (transform.position, transform.up*-1f);
		Debug.DrawRay (transform.position,transform.up*-1f);
		if (Physics.Raycast (GravityCheckingRay, out hit, gravityCheckDistance) ) {
			if (hit.collider.transform.parent != gameObject && hit.collider.transform.gameObject != gameObject) {
				gravityEnabled = false;
			} else {
				gravityEnabled = true;
			}
		} else {
			gravityEnabled = true;
		}
	}


}
