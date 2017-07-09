using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserRifle : Weapon {





	MovementController mc;

	float maxRange = 50f;
	float reloadTimer = 0f;

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
	}

	public override void AimDownSights ()
	{
		throw new System.NotImplementedException ();
	}

	public override void Fire(){
		CmdFire ();
	}

	public void CmdFire(){
		if (!canFire || ammo <= 0) {
			return;
		}
		Debug.DrawRay (transform.position, cam.ScreenPointToRay (Input.mousePosition).direction * maxRange, Color.red, 3f);
		RaycastHit hit;
		ammo--;
		if(Physics.Raycast(new Ray(transform.position,cam.ScreenPointToRay(Input.mousePosition).direction), out hit,maxRange))
			{
				if(hit.collider.gameObject.GetComponent<Player>())
					{
				mc.CmdDamage(damage,hit.collider.gameObject.GetComponent<Player>());
					}
			}
	}

	public override void Reload(){
		if(ammo != maxAmmo && clips > 0)
		canFire = false;
		reloadTimer = reloadTime;
		Debug.Log ("Reloading!");
	}




	// Update is called once per frame
	void Update () {
		if (!canFire) {
			if (reloadTimer > 0) {
				reloadTimer -= Time.deltaTime;
			}
			else if (reloadTimer <= 0) {
				Debug.Log ("Loaded!");
				ammo = maxAmmo;
				canFire = true;
				clips -= 1;
			}
		}
		text.text = ammo.ToString();
	}
}
