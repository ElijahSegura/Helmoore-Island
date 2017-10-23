using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : Item
{
    //in minutes
    public float growTime = 20f;
    public override GameObject getObject()
    {
        switch(itemName)
        {
            case "Carrot":
                return add.Carrot;
        }
        return null;
    }

    public override void Interact()
    {
        if(pickupable)
        {
            PickUp();
        }
    }

    void Start()
    {
        if(transform.parent != null)
        {
            setChar();
        }
        else
        {
            setChar();
            addE();
        }
    }

    public override void set(Item i)
    {
        //method not needed
    }
}
