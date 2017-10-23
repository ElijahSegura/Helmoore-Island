using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour {
    public GameObject gives;
    public Sprite icon;
    public Item[] required;
    public Forger Forger;
    public float delay = 4f;

    public int wood = 0;
    public int ironIngot = 0;
    public int copperIngot = 0;
    public int goldIngot = 0;
    public int silverIngot = 0;
    Type t;
    string o;
    void Start()
    {
        bool hasWood = false;
        List<Item> inventory = new List<Item>(getChar().getInventory());
        if (wood != 0)
        {
            t = typeof(WoodLog);
            if(inventory.FindAll(findItem).Count >= wood)
            {
                hasWood = true;
            }
        }

        if(ironIngot != 0)
        {
            o = "Iron Ingot";
            if (inventory.FindAll(findOre).Count >= ironIngot)
            {
                hasWood = true;
            }
        }
        
    }

    public bool findItem(Item i)
    {
        if(i.GetType() == t) {
            return true;
        }
        return false;
    }

    public bool findOre(Item i)
    {
        if(i.itemName.Equals(o))
        {
            return true;
        }
        return false;
    }

    public Character getChar()
    {
        return Camera.main.transform.parent.parent.gameObject.GetComponent<Character>();
    }

    public void Click()
    {
        getChar().getCamera().setForgingRecipe(this);
        Forger.set(gives, delay, required);
    }
}