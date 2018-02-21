using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : Item
{
    void Start()
    {
        setChar();
    }

    public override void Interact()
    {
        getCamera().openForge();
    }
    public override GameObject getObject()
    {
        return null;
    }
    public override void set(Item i)
    {

    }
}
