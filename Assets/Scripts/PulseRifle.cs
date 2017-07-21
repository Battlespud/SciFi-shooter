using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PulseRifle : Weapon {

    public Text text;

	public GameObject Projectile;

	float projectileForce = 100f;

    // Use this for initialization
    void Start () {
		name = "PulseRifle";
        damage = 0;
		maxAmmo = 120;
		ammo = maxAmmo;
		maxClips = 24;
		clips = maxClips;
		accuracy = .8f;
		cooldown = .05f;
		reloadTime = 1.5f;
		maxRange = 150f;
		LoadReferences ();
	}


    public override void AimDownSights ()
	{
		throw new System.NotImplementedException ();
	}

	public override void Fire(){
		if (!canFire || ammo <= 0) {
			return;
		}
		GameObject G = Instantiate (Projectile) as GameObject;
		G.transform.position = Muzzle.transform.position; 
		Rigidbody rb = G.GetComponent<Rigidbody> ();
		rb.AddForce (Muzzle.transform.forward * projectileForce);
        ammo--;
	//	OnFire ();
		HandleCooldown();
    }




}
