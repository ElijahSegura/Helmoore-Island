﻿using System.Collections;
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
        character = Camera.main.transform.parent.parent.GetComponent<Character>();
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

    void Update()
    {
        if(forging)
        {
            time += Time.deltaTime;
            if(time >= del)
            {
                for(int i = 0; i < used.wood; i++)
                {
                    foreach (Item item in character.getInventory())
                    {
                        if(item.itemName.Equals("Wood Log")) {
                            character.removeFromInventory(item);
                            break;
                        }
                    }
                }
                for (int i = 0; i < used.ironIngot; i++)
                {
                    foreach (Item item in character.getInventory())
                    {
                        if (item.itemName.Equals("Iron Bar"))
                        {
                            character.removeFromInventory(item);
                            break;
                        }
                    }
                }
                for (int i = 0; i < used.silverIngot; i++)
                {
                    foreach (Item item in character.getInventory())
                    {
                        if (item.itemName.Equals("Silver Bar"))
                        {
                            character.removeFromInventory(item);
                            break;
                        }
                    }
                }

                for (int i = 0; i < used.goldIngot; i++)
                {
                    foreach (Item item in character.getInventory())
                    {
                        if (item.itemName.Equals("Gold Bar"))
                        {
                            character.removeFromInventory(item);
                            break;
                        }
                    }
                }
                for (int i = 0; i < used.copperIngot; i++)
                {
                    foreach (Item item in character.getInventory())
                    {
                        if (item.itemName.Equals("Copper Bar"))
                        {
                            character.removeFromInventory(item);
                            break;
                        }
                    }
                }
                character.sendSystemMessage("Forged 1 thing");
                character.addToInventory(item.GetComponent<Item>());
                forging = false;
                foreach(Recipe recipe in FindObjectsOfType<Recipe>())
                {
                    recipe.refresh();
                }
                time = 0f;
            }
        }
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
