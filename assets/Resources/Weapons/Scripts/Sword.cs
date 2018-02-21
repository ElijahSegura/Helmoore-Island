using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon {
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag.Equals("Enemy"))
        {
            c.gameObject.GetComponent<Mob>().damage(damage);
        }
    }
}
