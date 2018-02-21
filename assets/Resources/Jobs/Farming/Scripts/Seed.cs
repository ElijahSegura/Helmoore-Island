using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Item
{
    public Crop GrownCrop;
    public override GameObject getObject()
    {
        return null;
    }

    public override void Interact()
    {
        PickUp();
    }

    public override void set(Item i)
    {
        
    }
}
