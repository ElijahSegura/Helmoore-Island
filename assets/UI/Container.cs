using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item
{
    public List<Item> containerItems = new List<Item>();
    public override void Interact()
    {
        setChar();
        openContainer(containerItems, gameObject);
    }

    public void removeFromContainer(Item i)
    {
        containerItems.Remove(i);
    }

    public void removeFromContainer(string i)
    {

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

    public bool hasItem(string i, int amount)
    {
        return false;
    }
}
