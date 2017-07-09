using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPistol : Weapon {







	float maxRange = 50f;

	// Use this for initialization
	void Start () {
		damage = 5;
		maxAmmo = 12;
		ammo = maxAmmo;
		maxClips = 6;
		clips = maxClips;
		accuracy = 1f;
		cooldown = .25f;
		reloadTime = 1f;
	}

	public override void AimDownSights ()
	{
		throw new System.NotImplementedException ();
	}

	public override void Fire(){
		if (!canFire || ammo <= 0) {
			return;
		}






	}

	public override void Reload(){

	}




	// Update is called once per frame
	void Update () {
		
	}
}
