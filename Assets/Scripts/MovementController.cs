using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;





public enum MoveSpeed{
	AIM = 0,
	WALK = 1,
	RUN = 2,
	SPRINT = 3
};

/*
public enum MoveSpeed{
	AIM = 4,
	WALK = 6,
	RUN = 8,
	SPRINT = 14
};

*/
public enum Stance{
	CROUCH = 0,
	SLIDE = 1,
	STAND = 2
};


public class MovementController : NetworkBehaviour {

	public static float[] SpeedStanceModifiers = new float[4]{ 1f, 1.5f, 2f, 3.5f };
	public static float[] SpeedStanceAccelerationModifiers = new float[4]{ .75f, 1f, 1.75f, 2f };

	public Animator ani;
	AudioListener Listener;
	Player player;
	Weapon weapon;
	Stance stance = Stance.STAND;
	bool isCrouching;

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





	//AccelerationBasedMovement
	public GameObject wayfinder;
	float xAcceleration = 5f;
	float zStartAcceleration = 3.5f; //starting acceleration
	float zAcceleration = 2f;//once we get moving we dont accelerate as fast
	float zVelocitySwitch = 4f;

	float MaxVelocityX = 4f;
	float MaxVelocityZ = 4f;
	float backwardsModifiersMaxSpeed = .4f;

	Vector3 Acceleration;
	Vector3 Velocity;



	public int jumpLimit = 1;

	bool WeaponDrawn = false;
	bool lockoutDraw = false;
	bool AimingSights =false;
	bool lockoutAim = false;

	bool lockout = false;

	//Animation
	int jumpHash = Animator.StringToHash("Jump");

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
	float gravityCheckOffsetDistance = .11f;
	float gravityCheckDistance;
	public bool gravityEnabled;
	public float currGravity;
	float gravityTimer;
	float mGravityTimer;
	float Interval = 20f;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			stance = Stance.STAND;
			Listener = GetComponent<AudioListener> ();
			player = GetComponent<Player> ();
			Cursor.SetCursor (cursor, new Vector2(16,16), CursorMode.ForceSoftware);
			GrabReferences ();
			moveSpeed = MoveSpeed.WALK;
			ToggleCursorVisible ();
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
			CatchInput ();
			HandleAnimation ();
			GenerateVelocityVector ();
			Move ();
		}
	}

	public void CmdDamage(int i, Player p){
		p.DoDamageTest (i);
	}

	void CatchInput(){
		if (!lockout)
		MovementInput ();
		ActionsInput ();
		SystemInput ();
		WeaponInput ();

	}

	void MovementInput(){
		
		if (Input.GetKey (KeyBinds.Forward)){
			toMove.z += 1;
			if(wayfinder.transform.InverseTransformVector(Velocity).z < zVelocitySwitch){
				Acceleration.z += zStartAcceleration*SpeedStanceAccelerationModifiers[(int)moveSpeed]*Time.deltaTime;
			}
			else{
				Acceleration.z += zAcceleration*SpeedStanceAccelerationModifiers[(int)moveSpeed]*Time.deltaTime;
			}
	}
		if (Input.GetKey (KeyBinds.Back)){
			toMove.z += -1;
			Acceleration.z -= zAcceleration*SpeedStanceAccelerationModifiers[(int)moveSpeed]*Time.deltaTime;
	}
		if (Input.GetKey (KeyBinds.Left)){
			toMove.x += -1;
			Acceleration.x -= xAcceleration*SpeedStanceAccelerationModifiers[(int)moveSpeed]*Time.deltaTime;
	}
		if (Input.GetKey (KeyBinds.Right)) {
			toMove.x += 1;
			Acceleration.x += xAcceleration*SpeedStanceAccelerationModifiers[(int)moveSpeed]*Time.deltaTime;
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			ChangeStance (Stance.SLIDE);
		}

	}

	void ActionsInput(){
		if (Input.GetKeyDown (KeyBinds.Jump))
			Jump ();
		if (Input.GetKeyDown (KeyBinds.Crouch)) {
			Crouch ();
		}
	}

	void SystemInput(){
		if (Input.GetKeyDown (KeyCode.G)) 
			ToggleCursorVisible ();
		if (Input.GetKeyDown (KeyCode.Escape)) 
			Unlock ();
	}

	void WeaponInput(){
		if (Input.GetKeyDown (KeyBinds.Reload)) 
			weapon.Reload ();
		if (Input.GetMouseButtonDown (KeyBinds.LeftClick))
		if (!weapon.automatic) {
			if (WeaponDrawn && !weapon.automatic) {
				weapon.Fire ();
			} else {
				ToggleHolster ();
			}
		}
		if (Input.GetMouseButton (KeyBinds.LeftClick))
		if (weapon.automatic) {
			if (WeaponDrawn) {
				weapon.Fire ();
			} else {
				ToggleHolster ();
			}
		}
		if (Input.GetMouseButtonDown(KeyBinds.RightClick))
			ToggleSights();
		if (Input.GetKey (KeyBinds.Sprint) && !isCrouching) {
			if (AimingSights)
				ToggleSights ();
			if (!AimingSights && (player.Stamina > 2 || (player.Stamina > 0 && player.usingStamina))) {
				moveSpeed = MoveSpeed.SPRINT;
				player.usingStamina = true;
				player.Stamina -= Time.deltaTime;
			} else {
				player.usingStamina = false;
				if (AimingSights) 	moveSpeed = MoveSpeed.AIM;
				else{	moveSpeed = MoveSpeed.RUN;	}
			}
		}else {
			player.usingStamina = false;
			if (AimingSights) 	moveSpeed = MoveSpeed.AIM;
			else{	moveSpeed = MoveSpeed.RUN;	}
		}
		if (Input.GetKeyDown(KeyBinds.ToggleWeapon))
			ToggleHolster();
	}


	void Crouch(){
		switch (stance) {
		case(Stance.STAND):{
				if (moveSpeed == MoveSpeed.RUN ) {
					ChangeStance (Stance.SLIDE);
				} else {
					ChangeStance (Stance.CROUCH);
				}
				break;
			}
		case(Stance.CROUCH):{
				ChangeStance (Stance.STAND);
				break;
			}
		case(Stance.SLIDE):{

				break;
			}

		}

	}

	void ChangeStance(Stance toStance)
	{
		switch (toStance) {
		case(Stance.STAND):{
				stance = Stance.STAND;
				moveSpeed = MoveSpeed.RUN;
				isCrouching = false;
				lockout = false;
			///	transform.localScale = new Vector3 (1f, 2f, 1f);
				break;
			}
		case(Stance.CROUCH):{
				stance = Stance.CROUCH;
				moveSpeed = MoveSpeed.WALK;
				isCrouching = true;
		///		transform.localScale = new Vector3 (1f, 1.5f, 1f);
				lockout =   false;
				break;
			}
		case(Stance.SLIDE):{
				stance = Stance.SLIDE;
				Debug.Log("Sliding!");
				isCrouching = true;
				lockout = true;
		///		transform.localScale = new Vector3(1f, 1f, 1f);
				StartSlide ();
				break;
			}

		}

	}

	void StartSlide(){
		StartCoroutine (SlideHandler());
	}

	private IEnumerator SlideHandler(){
		ani.SetTrigger ("SlideTrigger");
		gravityEnabled = true;
		float slideSpeed = Velocity.magnitude*1.8f;
		Debug.Log (slideSpeed);
		Vector3 directionVector = Velocity.normalized;//transform.TransformDirection(transform.forward);
		while (slideSpeed > 3) {
			gravityEnabled = true;
			controller.Move(directionVector.normalized*slideSpeed*Time.deltaTime);
			slideSpeed = Mathf.Lerp (slideSpeed, 0f, .012f);
			yield return null;
		}
		Debug.Log ("End slide");
		ani.ResetTrigger ("SlideTrigger");
		ChangeStance (Stance.STAND);
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
			ToggleCursorVisible ();
			lockoutDraw = true;
			StartCoroutine (ShiftPosition (camDefaultPosition, .5f));
		} else if(!lockoutDraw) {
			WeaponDrawn = true;
			ToggleCursorVisible ();
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



	void ToggleCursorVisible(){
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

	private void CheckGravity(){ 
		RaycastHit[] hit;
		Debug.DrawRay (transform.position, Vector3.down * gravityCheckDistance); 
		Ray GravityCheckingRay = new Ray (transform.position, transform.up*-1f);
		//Debug.DrawRay (transform.position,transform.up*-1f,Color.green,2f);
		hit =Physics.RaycastAll (transform.position, transform.up*-1, gravityCheckDistance); 
			gravityEnabled = true;
			foreach (RaycastHit h in hit) {
				if (h.collider.gameObject != gameObject) {
					gravityEnabled = false;
		}
	}
}


	void HandleAnimation(){
		if (Velocity.normalized.z != 0f) {
			ani.SetBool ("running", true);
		} else {
			ani.SetBool ("running", false);
		}
		ani.Play ("Jump",-1,0f);
	}

	void Move(){
		controller.Move (Velocity*Time.deltaTime);
		if (toJump) {
			AddImpact (transform.up, jumpForce);
			toJump = false;

		}
	}

	void GenerateVelocityVector(){
		Vector3 Friction = new Vector3 (0f, 0f, 0f);

		float friction = .45f;


		Acceleration = wayfinder.transform.TransformVector (Acceleration);

		Vector3 VelocityLocalized = wayfinder.transform.InverseTransformVector (Velocity);

//		Debug.Log (toMove.x);

		if (toMove.x == 0) {
			VelocityLocalized.x = 0;
		} else if (Acceleration.x == 0) {
		//	VelocityLocalized.x = Mathf.Lerp (VelocityLocalized.x, 0f, 5f*friction);
		}

		if (Mathf.Abs (VelocityLocalized.z) < 3f && Acceleration.z == 0) {
			VelocityLocalized.z = 0;
		} else if (Acceleration.z == 0) {
			VelocityLocalized.z = Mathf.Lerp (VelocityLocalized.z, 0f, friction);
		}

		float dotProd = Vector3.Dot (wayfinder.transform.forward, Velocity);

		VelocityLocalized.x = Mathf.Clamp (VelocityLocalized.x, MaxVelocityX * -1, MaxVelocityX*SpeedStanceModifiers[(int)moveSpeed]);
		VelocityLocalized.z = Mathf.Clamp (VelocityLocalized.z, MaxVelocityZ * -1, MaxVelocityZ*SpeedStanceModifiers[(int)moveSpeed]);

		Velocity = wayfinder.transform.TransformVector (VelocityLocalized);

		Velocity += Acceleration;// + Friction;
		Acceleration = new Vector3 ();

	}


}
