using UnityEngine;

public class Crop : Item
{
    //in minutes
    public Seed seed;
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
        harvest();
    }

    void Start()
    {
        setChar();
    }

    private void harvest()
    {
        //gives "n" of crop and "n" of seeds
        for (int i = 0; i < Random.Range(0,5); i++)
        {
            getChar().addToInventory(this);
        }
        for (int i = 0; i < Random.Range(0, 1); i++)
        {
            getChar().addToInventory(seed);
        }
        PickUp();
    }

    public override void set(Item i)
    {
        //method not needed
    }
}
