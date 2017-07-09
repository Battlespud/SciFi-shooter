using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public int damage;
	public int maxAmmo;
	public int ammo;
	public int clips;
	public int maxClips;
	public float accuracy;

	public float cooldown;
	public float reloadTime;


	public abstract void Fire ();
	public abstract void Reload();
	public abstract void AimDownSights();



}
