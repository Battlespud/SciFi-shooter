using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour {

	//Rotation
	public GameObject Pivot;

	//Sound
	public AudioClip FireSound;
	public AudioClip ReloadSound;

	public AudioSource WeaponSoundSource;

	public Light MuzzleFlash;

	public MovementController movementController;
	public Camera cam;


	public virtual void LoadReferences(){
		movementController = transform.parent.GetComponentInParent<MovementController> ();
		cam = Camera.main;
		Pivot = gameObject.transform.parent.gameObject;

		//Light
		MuzzleFlash = GetComponentInChildren<Light> ();
		MuzzleFlash.enabled = false;

		//Sound
		WeaponSoundSource = GetComponent<AudioSource>();
		//Debug.Log (string.Format ("Sounds/Weapons/{0}/{1}FireSound", name, name));
		FireSound = Resources.Load<AudioClip> (string.Format("Sounds/Weapons/{0}/{1}FireSound",name ,name));
		ReloadSound = Resources.Load<AudioClip> (string.Format("Sounds/Weapons/{0}/{1}ReloadSound",name,name,ReloadSound.ToString()));

		AmmoText = GetComponentInChildren<Text> ();
	}

	public virtual void Reload(){
		if(ammo != maxAmmo && clips > 0)
			canFire = false;
		reloadTimer = reloadTime;
		//	Debug.Log ("Reloading!");
	}

	public virtual void Fire(){
		if (ammo > 0) {
			WeaponSoundSource.PlayOneShot (FireSound);
			ammo--;
		}
		Debug.Log ("You need to write a custom fire method for this weapon still");
	}

	public string name;

	public Text AmmoText;

	public float maxRange;

	public int damage;
	public int maxAmmo;
	public int ammo;
	public int clips;
	public int maxClips;
	public float accuracy;

	//between shots
	public float cooldownTimer;
	public float cooldown;
	public bool canFire;

	public float reloadTime;
    public float reloadTimer;



	public virtual void AimDownSights(){
		//TODO
	}


	public virtual void Update () {
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
		try{
		Pivot.transform.rotation = Quaternion.LookRotation (cam.ScreenPointToRay (Input.mousePosition).direction);
		}
		catch{
			//I literally dont care. it'll work.
		}
		AmmoText.text = ammo.ToString();
	}
}
