using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{

    [SyncVar] public int HP;
    public int damageAmount;

    // Update is called once per frame
    void Update()
    {
        Damage(damageAmount);
    }

    public void Damage(int amount)
    {
        if (!isServer)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(string.Format("HP was {0}; Applied {1} damage points; HP is {2}", HP, amount, HP - amount));
            HP -= amount;
        }

        if (HP <= 0)
        {
            HP = 0;
            Debug.Log("Dead");
        }
    }
}
