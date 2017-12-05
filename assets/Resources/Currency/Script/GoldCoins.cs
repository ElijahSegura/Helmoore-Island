using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoins : Item
{
    public int amount;
    void Start()
    {
        setChar();
    }

    public override GameObject getObject()
    {
        return null;
    }

    public override void Interact()
    {
        getChar().addGold(amount);
        Destroy(gameObject);
    }

    public override void set(Item i)
    {
    }
}
