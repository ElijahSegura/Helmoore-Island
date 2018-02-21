using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour {
    public GameObject rightHand, LeftHand, Sword, Shield;
    public void startAttack()
    {
        Sword.GetComponent<WeaponTrail>().setCombat(true);
    }

    public void endAttack()
    {
        GetComponent<Animator>().SetBool("Attack", false);
        Sword.GetComponent<WeaponTrail>().setCombat(false);
        foreach (Collider c in Sword.GetComponents<Collider>())
        {
            c.enabled = false;
        }
    }

    public void attack()
    {
        foreach (Collider c in Sword.GetComponents<Collider>())
        {
            c.enabled = true;
        }
    }
}
