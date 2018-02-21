using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCrystal : Item {
    public override GameObject getObject()
    {
        throw new NotImplementedException();
    }

    public override void Interact()
    {
        PickUp();
    }

    public override void set(Item i)
    {
        throw new NotImplementedException();
    }
    
}
