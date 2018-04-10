using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forger : MonoBehaviour {
    private GameObject item;
    private float time = 0f;
    private float del;
    private Character character;
    private Recipe used;
    void Start()
    {
        //character = Camera.main.transform.parent.parent.GetComponent<Character>();
    }

	public void set(GameObject obj, float delay, Recipe u)
    {
        this.item = obj;
        this.del = delay;
        this.used = u;
    }

    private bool forging = false;
    public void forge()
    {
        forging = true;
    }

    Anvil anvil;
    public void setAnvil(Anvil a)
    {
        this.anvil = a;
    }
}
/*

    foreach (Item item in used)
                {
                    foreach (Item initem in character.getInventory())
                    {
                        if(initem.itemName.Equals(item.itemName))
                        {
                            character.removeFromInventory(initem);
                            break;
                        }
                    }
                }
    */
