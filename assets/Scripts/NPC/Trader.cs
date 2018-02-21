using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : NPC
{
    public List<Item> items;
    public override void Interact()
    {
        FindObjectOfType<PlayerCamera>().openStall(items);
    }
}
