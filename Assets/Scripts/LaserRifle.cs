using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserRifle : Weapon {

	const float LaserTrail = .09f;

	public AudioClip LaserSound;
	public AudioSource FiringSound;
	public GameObject lrPrefab;
	public GameObject muzzle;

	Light MuzzleFlash;

	MovementController mc;

	float maxRange = 500f;

	//sanity check, prevents infinite loop
	int maxBounces = 3;
	int bounces = 0;


	public Text text;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		damage = 20;
		maxAmmo = 12;
		ammo = maxAmmo;
		maxClips = 6;
		clips = maxClips;
		accuracy = 1f;
		cooldown = .25f;
		reloadTime = 1f;
		canFire = true;
		mc = GetComponentInParent<MovementController> ();
		MuzzleFlash = GetComponentInChildren<Light> ();
		MuzzleFlash.enabled = false;
		FiringSound=GetComponent<AudioSource> ();
		LaserSound = Resources.Load < AudioClip> ("LaserSound");
	}

	public override void AimDownSights ()
	{
		throw new System.NotImplementedException ();
	}

	public override void Fire(){
		bounces = 0;
		CmdFire ();
	}

	public void CmdFire(){
		if (!canFire || ammo <= 0) {
			return;
		}
	//	Debug.DrawRay (muzzle.transform.position, cam.ScreenPointToRay (Input.mousePosition).direction * maxRange, Color.red, 3f);
		RaycastHit hit;
		Ray firingRay = new Ray (muzzle.transform.position, cam.ScreenPointToRay (Input.mousePosition).direction);
		ammo--;
		FiringSound.PlayOneShot (LaserSound);
		if (Physics.Raycast (firingRay, out hit, maxRange)) {
			if (hit.collider.gameObject.GetComponent<Player> ()) {
				mc.CmdDamage (damage, hit.collider.gameObject.GetComponent<Player> ());
				StartCoroutine (LineRendererHandlerFirstShot (hit.point));
			} else {
				if (hit.point != null) {
					StartCoroutine (LineRendererHandlerFirstShot (hit.point));
					if(hit.collider.gameObject.CompareTag("Reflective")){
						BounceShot (hit, firingRay);
					}
				}
			}
		} else {
			StartCoroutine (LineRendererHandlerFirstShot (muzzle.transform.position + (cam.ScreenPointToRay (Input.mousePosition).direction * maxRange)));
		}

	}

	void BounceShot(RaycastHit hit, Ray firingRay){
		if (bounces > maxBounces) {
			return;
		}
		bounces++;
	//	Debug.Log ("Bounce laser");
		//StartCoroutine (LineRendererHandler (hit.point,Vector3.Reflect(firingRay.direction,hit.normal)*maxRange));
	//	Debug.DrawRay (hit.transform.position, Vector3.Reflect(firingRay.direction,hit.normal)*maxRange, Color.red, 3f);
		Ray newFiringRay = new Ray(hit.transform.position, Vector3.Reflect(firingRay.direction,hit.normal));
		RaycastHit newHit;
		if (Physics.Raycast (newFiringRay, out newHit, maxRange)) {
			if (newHit.collider.gameObject.GetComponent<Player> ()) {
				mc.CmdDamage (damage, newHit.collider.gameObject.GetComponent<Player> ());
				StartCoroutine (LineRendererHandler (hit.point, newHit.point));
			} else {			//not player
				if (newHit.point != null) {
					StartCoroutine (LineRendererHandler (hit.point,newHit.point));
					if (hit.collider.gameObject.CompareTag ("Reflective")) {
						BounceShot (newHit, newFiringRay);
					}
				} else {
					StartCoroutine (LineRendererHandler (hit.point, Vector3.Reflect (firingRay.direction, hit.normal) * maxRange));
				}
			}
		} else {
			StartCoroutine (LineRendererHandler (hit.point, Vector3.Reflect (firingRay.direction, hit.normal) * maxRange));

		}
		Debug.Log (bounces);
	}

	private IEnumerator LineRendererHandlerFirstShot(Vector3 endPos){
		MuzzleFlash.enabled = true;
		GameObject renderer = Instantiate (lrPrefab);
		LineRenderer lr = renderer.GetComponent<LineRenderer> ();
		MuzzleFlash.color = Color.cyan;
		lr.startColor = Color.cyan;
		lr.endColor = Color.cyan;
		renderer.transform.position = muzzle.transform.position;
		float time = LaserTrail;
		float elapsedTime = 0f;
		lr.enabled = true;
		lr.SetPositions(new Vector3[2]{muzzle.transform.position,endPos});
		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		MuzzleFlash.enabled = false;
		Destroy (renderer);
	}

	private IEnumerator LineRendererHandler(Vector3 startPos, Vector3 endPos){
		Debug.Log ("Bounce laser");
		GameObject renderer = Instantiate (lrPrefab);
		LineRenderer lr = renderer.GetComponent<LineRenderer> ();
		lr.startColor = Color.red;
		lr.endColor = Color.red;
		float time = LaserTrail;
		float elapsedTime = 0f;
		lr.enabled = true;
		lr.SetPositions(new Vector3[2]{startPos,endPos});
		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		Destroy (renderer);
	}

	public override void Reload(){
		if(ammo != maxAmmo && clips > 0)
		canFire = false;
		reloadTimer = reloadTime;
	//	Debug.Log ("Reloading!");
	}




	// Update is called once per frame
	void Update () {
		if (!canFire) {
			if (reloadTimer > 0) {
				reloadTimer -= Time.deltaTime;
			}
			else if (reloadTimer <= 0) {
			//	Debug.Log ("Loaded!");
				ammo = maxAmmo;
				canFire = true;
				clips -= 1;
			}
		}
		text.text = ammo.ToString();
	}
}
