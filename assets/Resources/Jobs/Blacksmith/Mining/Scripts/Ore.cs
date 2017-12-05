
using UnityEngine;

public class Ore : Item {
    public float purity = 1f;
    private bool generated = false;
    

    public void genOre()
    {
        purity = Random.Range(0.5f, 0.9f);
        gen();
        generated = true;
    }

    public void genPureOre()
    {
        purity = Random.Range(0.85f, 1f);
        gen();
        generated = true;
    }

    public void gen()
    {
        value = (int)Mathf.Floor(maxValue * purity);
    }

    public override void Interact()
    {
        PickUp();
    }

    public override GameObject getObject()
    {
        switch(itemName)
        {
            case "Copper Ore":
                return add.CopperOre;
            case "Iron Ore":
                return add.IronOre;
            case "Silver Ore":
                return add.SilverOre;
            case "Gold Ore":
                return add.GoldOre;
        }
        return null;
    }
    public override void set(Item i)
    {
        Ore o = (Ore)i;
        this.value = o.value;
        this.maxValue = o.maxValue;
        this.itemName = o.itemName;
        this.purity = o.purity;
        this.generated = o.generated;
    }
}
