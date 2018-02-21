using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Recipe : MonoBehaviour {
    public GameObject gives;
    public Sprite icon;
    public Forger Forger;
    public float delay = 4f;

    public int wood = 0;
    public int ironIngot = 0;
    public int copperIngot = 0;
    public int goldIngot = 0;
    public int silverIngot = 0;
    Type t;
    bool hasWood = false;
    bool hasIron = false;
    string o;
    void OnEnable()
    {
        hasWood = false;
        hasIron = false;
        List<Item> inventory = new List<Item>(getChar().getInventory());
        if (wood != 0)
        {
            t = typeof(WoodLog);
            if (inventory.FindAll(findItem).Count >= wood)
            {
                hasWood = true;
            }
        }

        if (ironIngot != 0)
        {
            o = "Iron Bar";
            if (inventory.FindAll(findOre).Count >= ironIngot)
            {
                hasIron = true;
            }
        }
        if (!((wood > 0 == hasWood) && (ironIngot > 0 == hasIron)))
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
    }

    void Start()
    {
        hasWood = false;
        hasIron = false;
        List<Item> inventory = new List<Item>(getChar().getInventory());
        if (wood != 0)
        {
            t = typeof(WoodLog);
            if (inventory.FindAll(findItem).Count >= wood)
            {
                hasWood = true;
            }
        }

        if (ironIngot != 0)
        {
            o = "Iron Bar";
            if (inventory.FindAll(findOre).Count >= ironIngot)
            {
                hasIron = true;
            }
        }
        if (!((wood > 0 == hasWood) && (ironIngot > 0 == hasIron)))
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
    }

    public void refresh()
    {
        hasWood = false;
        hasIron = false;
        List<Item> inventory = new List<Item>(getChar().getInventory());
        if (wood != 0)
        {
            t = typeof(WoodLog);
            if (inventory.FindAll(findItem).Count >= wood)
            {
                hasWood = true;
            }
        }

        if (ironIngot != 0)
        {
            o = "Iron Bar";
            if (inventory.FindAll(findOre).Count >= ironIngot)
            {
                hasIron = true;
            }
        }
        if (!((wood > 0 == hasWood) && (ironIngot > 0 == hasIron)))
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
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
        Forger.set(gives, delay, GetComponent<Recipe>());
        getChar().getCamera().setForgingRecipe(this);
    }
}