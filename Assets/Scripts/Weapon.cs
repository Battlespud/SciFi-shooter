using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour {

	//Rotation
	public GameObject Pivot;

	//Position of shots
	public GameObject Muzzle;

	//Sound
	public AudioClip FireSound;
	public AudioClip ReloadSound;

	public AudioSource WeaponSoundSource;

	public Light MuzzleFlash;

	public Camera cam;




	public virtual void LoadReferences(){
		cam = Camera.main;

		//Muzzle

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
		if (ammo != maxAmmo && clips > 0) {
			reload = true;
			ammo = 0;
			reloadTimer = reloadTime;
			try {
				WeaponSoundSource.PlayOneShot (ReloadSound);
			} catch {
				Debug.Log ("This weapon needs a reload sound. " + name);
			}
		}
	}

	public virtual void Fire(){
		if (ammo > 0 && canFire) {
			try{WeaponSoundSource.PlayOneShot (FireSound);}catch{Debug.Log ("This weapon needs a firing sound. " + name);}
			ammo--;
			OnFire ();
		}
		Debug.Log ("You need to write a custom fire method for this weapon still");
	}

	public virtual void OnFire(){
		MuzzleFlash.enabled = true;
		Invoke ("EndMuzzleFlash", cooldown);
	}

	public virtual void HandleCooldown(){
		canFire = false;
		Invoke("CheckCooldown",cooldown);
	}

	public virtual void CheckCooldown(){
		canFire = true;
	}

	public void EndMuzzleFlash(){
		MuzzleFlash.enabled = false;
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

	public bool automatic = true;
	bool reload = false;

	//between shots
	public float cooldownTimer;
	public float cooldown;
	public bool canFire=true;


	public float reloadTime;
    public float reloadTimer;



	public virtual void AimDownSights(){
		//TODO
	}


	public virtual void Update () {
		if (reload) {
			if (reloadTimer > 0) {
				reloadTimer -= Time.deltaTime;
			} else if (reloadTimer <= 0) {
				//	Debug.Log ("Loaded!");
				ammo = maxAmmo;
				clips -= 1;
				reload = false;
			}
		}
		try{
			if(Pivot!=null){
		//Pivot.transform.rotation = Quaternion.LookRotation (cam.ScreenPointToRay (Input.mousePosition).direction); //even when null this somehow causes an issue, idgi
			}
		}
		catch{
			//I literally dont care. it'll work.
		}
		AmmoText.text = ammo.ToString();
		Debug.DrawRay (Muzzle.transform.position, Muzzle.transform.forward * maxRange, Color.yellow);

	}
}
