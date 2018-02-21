using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLog : Item
{

    void Start()
    {
        setChar();
        stackable = true;
        name = itemName;
    }

    public override void Interact()
    {
        PickUp();
    }

    public override void set(Item i)
    {
        
    }

    public override GameObject getObject()
    {
        return add.WoodLog;
    }
}
