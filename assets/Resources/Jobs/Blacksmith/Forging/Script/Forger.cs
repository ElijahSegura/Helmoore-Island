using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forger : MonoBehaviour {
    private GameObject item;
    private float time = 0f;
    private float del;
    private Character character;
    private Item[] used;
    void Start()
    {
        character = transform.parent.parent.parent.GetComponent<Character>();
    }

	public void set(GameObject obj, float delay, Item[] u)
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

    void Update()
    {
        if(forging)
        {
            time += Time.deltaTime;
            if(time >= del)
            {
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
                character.addToInventory(item.GetComponent<Item>());
                forging = false;
                time = 0f;
            }
        }
    }
}
