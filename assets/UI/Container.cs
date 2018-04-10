using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item
{
    public List<Item> containerItems = new List<Item>();
    public int startingWood;

    public Item wood;
    void Start()
    {
        if(startingWood > 0)
        {
            for (int i = 0; i < startingWood; i++)
            {
                containerItems.Add(wood);
            }
        }
    }

    public override void Interact()
    {
        setChar();
        openContainer(containerItems, gameObject);
    }

    public void removeFromContainer(Item i)
    {
        containerItems.Remove(i);
    }

    public void removeFromContainer(String i)
    {
        foreach (Item item in containerItems)
        {
            if(item.itemName.Equals(i))
            {
                removeFromContainer(item);
                break;
            }
        }
    }

    public List<Item> getItems()
    {
        return containerItems;
    }

    public void addToContainer(Item i)
    {
        containerItems.Add(i);
    }
    public override GameObject getObject()
    {
        return null;
    }
    public override void set(Item i)
    {

    }

    internal bool hasItem(string v, int amount)
    {
        int temp = 0;
        foreach (Item item in containerItems)
        {
            if(item.itemName.Equals(v))
            {
                temp++;
                if(temp >= amount)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
