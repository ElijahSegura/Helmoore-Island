using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Architecture : Item
{
    public int wood, clay, glass, steel, iron, dirt;
    private int numWood = 0, numClay = 0, numGlass = 0, numSteel = 0, numIron = 0, numDirt = 0;
    public bool canPlace()
    {
        setChar();
        int needs = 0;
        int has = 0;
        if (wood > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if(item.itemName.Equals("Wood Log"))
                {
                    numWood++;
                    if(numWood>= wood)
                    {
                        has++;
                        break;
                    }
                }
            }
        }
        if(clay > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if (item.itemName.Equals("Clay"))
                {
                    numClay++;
                    if (numClay >= clay)
                    {
                        has++;
                        break;
                    }
                }
            }
        }
        if(glass > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if (item.itemName.Equals("Glass"))
                {
                    numGlass++;
                    if (numGlass >= glass)
                    {
                        has++;
                        break;
                    }
                }
            }
        }
        if(steel > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if (item.itemName.Equals("Steel Bar"))
                {
                    numSteel++;
                    if (numSteel >= steel)
                    {
                        has++;
                        break;
                    }
                }
            }
        }
        if(iron > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if (item.itemName.Equals("Iron Bar"))
                {
                    numIron++;
                    if (numIron >= iron)
                    {
                        has++;
                        break;
                    }
                }
            }
        }
        if(dirt > 0)
        {
            needs++;
            foreach (Item item in getChar().getInventory())
            {
                if (item.itemName.Equals("Dirt"))
                {
                    numDirt++;
                    if (numDirt >= dirt)
                    {
                        has++;
                        break;
                    }
                }
            }
        }

        
        if(has == needs)
        {
            return true;
        }


        return false;
    }

    private bool canFit = true;
    public bool canfitHere()
    {
        return false;
    }

    public override GameObject getObject()
    {
        throw new NotImplementedException();
    }

    public override void Interact()
    {
        throw new NotImplementedException();
    }

    public override void set(Item i)
    {
        throw new NotImplementedException();
    }
}
