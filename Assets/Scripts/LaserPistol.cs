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
        Debug.DrawRay(transform.position, cam.ScreenPointToRay(Input.mousePosition).direction * maxRange, Color.red, 3f);
        ammo--;
    }

	public override void Reload(){
        //press R
        if (ammo != maxAmmo && clips > 0)
        canFire = false;
        reloadTimer = reloadTime;
        Debug.Log("Reloading!");
    }




	// Update is called once per frame
	void Update () {
	}
}
