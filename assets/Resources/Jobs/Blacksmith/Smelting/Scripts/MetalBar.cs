using System;
using UnityEngine;

public class MetalBar : Item
{
    public Ore ore;
    public float delay;
    public override void Interact()
    {
        PickUp();
    }
    public override GameObject getObject()
    {
        switch (itemName)
        {
            case "Copper Bar":
                return add.CopperBar;
            case "Iron Bar":
                return add.IronBar;
            case "Silver Bar":
                return add.SilverBar;
            case "Gold Bar":
                return add.GoldBar;
        }
        return null;
    }
    public override void set(Item i)
    {

    }
}
