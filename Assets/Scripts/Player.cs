using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{


	public Text healthText;
	public int MaxHP = 100;
    [SyncVar] public int HP;
	//in seconds
	[SyncVar] public float MaxStamina = 5f;
	[SyncVar] public float Stamina = 5f;
	//in seconds, how long to fill bar
	float staminaRegenRate = 8f;
	float delay = 1.5f;
	float delayCounter = 0f;
	public bool usingStamina = false;
	public int damageAmount;


	void Start(){
		if (isLocalPlayer) {
			Stamina = MaxStamina;
			HP = MaxHP;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (isLocalPlayer) {
			RegenStamina ();

		}
		if (HP < MaxHP) {
			healthText.text = HP.ToString ();
		}
		else {
			healthText.text = "";
		}
    }

	public void DoDamageTest(int i){
		Debug.Log(string.Format("HP was {0}; Applied {1} damage points; HP is {2}", HP, i, HP - i));
		HP -= i;
	if (HP <= 0)
	{
		HP = 0;
		Debug.Log("Dead");
	}	}

    public void Damage(int amount)
    {
     //   if (!isServer)
       //     return;
		HP -= amount;
		if (HP < 0) {
			HP = 0;
		}


    }

	void RegenStamina(){
		if (!usingStamina) {
			if (delayCounter <= 0f && Stamina < MaxStamina) {
				Stamina += MaxStamina / staminaRegenRate * Time.deltaTime;
			} else {
				delayCounter -= Time.deltaTime;
			}
		} else {
			delayCounter = delay;
		}
	}

}
