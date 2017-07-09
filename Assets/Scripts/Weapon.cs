using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public Camera cam;

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


	public abstract void Fire ();
	public abstract void Reload();
	public abstract void AimDownSights();



}
