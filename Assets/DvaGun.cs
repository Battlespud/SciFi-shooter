using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DvaGun : Weapon{

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
	
	// Update is called once per frame
	void Update () {
		
	}
}
