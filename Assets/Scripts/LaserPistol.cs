using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LaserPistol : Weapon {
    UnityEvent HUDUpdate = new UnityEvent();
    List<string> HUDInfo;

    float maxRange = 25f;
    //float reloadTimer = 0f;

    public Text text;

    // Use this for initialization
    void Start () {
        HUDInfo = new List<string>();
        HUDUpdate.AddListener(OnFire);
        damage = 5;
		maxAmmo = 12;
		ammo = maxAmmo;
		maxClips = 6;
		clips = maxClips;
		accuracy = 1f;
		cooldown = .25f;
		reloadTime = 1f;
        GetHUDInfo("HP: " + GetComponentInParent<Player>().HP, "Clips: " + clips, string.Format("Ammo: {0}/{1}", ammo, maxAmmo));
	}

    // Update is called once per frame
    void Update()
    {
        if (!canFire)
        {
            if (reloadTimer > 0)
            {
                reloadTimer -= Time.deltaTime;
            }
            else if (reloadTimer <= 0)
            {
                Debug.Log("Loaded!");
                ammo = maxAmmo;
                canFire = true;
                clips -= 1;
            }
        }
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
        OnFire();
    }

	public override void Reload(){
        //press R
        if (ammo != maxAmmo && clips > 0)
        canFire = false;
        reloadTimer = reloadTime;
        Debug.Log("Reloading!");
    }

    private void GetHUDInfo(params object[] info)
    {
        foreach (string str in info)
        {
            HUDInfo.Add(str.ToString() + "\n");
        }
    }

    private void OnFire()
    {
        text.text = string.Empty;
        foreach (string str in HUDInfo)
        {
            text.text += str;
        }
    }
}
