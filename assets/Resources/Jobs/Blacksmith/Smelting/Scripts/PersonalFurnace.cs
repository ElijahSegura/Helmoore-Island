using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalFurnace : Item
{
    private float time = 0f;
    private float delay = 3f;
    private bool smelting = false;
    private int max = 0;
    private int current = 0;
    public GameObject defaultBar;
    public GameObject CurrentBar;

    private List<Item> sortedOres = new List<Item>();
    

    void Start()
    {
        setChar();
        delay = defaultBar.GetComponent<MetalBar>().delay;
    }

    public void changeBar(GameObject bar)
    {
        this.CurrentBar = bar;
        getOres();
        this.delay = CurrentBar.GetComponent<MetalBar>().delay;
        getCamera().setSmelting(CurrentBar.GetComponent<MetalBar>());
    }

    public void setMax(int max)
    {
        this.max = max;
    }

    public override void Interact()
    {
        getChar().openUI();
        getCamera().openSmelting();
        getCamera().setFurnace(this);
        getCamera().setSmelting(defaultBar.GetComponent<MetalBar>());
        getOres();
    }

    private void getOres()
    {
        sortedOres.Clear();
        int length = 0;
        List<Ore> ores = new List<Ore>();
        foreach(Item item in getChar().getInventory())
        {
            if(item.itemName.Equals(CurrentBar.GetComponent<MetalBar>().ore.itemName))
            {
                ores.Add((Ore)item);
            }
        }




        for (int i = 0; i < length; i++)
        {
            float max = 0f;
            Ore temp = null;
            int o = 0;
            int p = 0;
            foreach (Item item in ores)
            {
                Debug.Log(item);
                if (((Ore)item).purity > max)
                {
                    max = ((Ore)item).purity;
                    temp = (Ore)item;
                    p = o;
                }
                o++;
            }
            sortedOres.Add(temp);
            Debug.Log(temp);
            ores.Remove(temp);
        }





    }

    public void startSmelting()
    {
        smelting = true;
    }

    private List<Item> smelted = new List<Item>();
    float l = 0f;
    public void Update()
    {
        if (smelting)
        {
            time += Time.deltaTime;
            if(time >= delay)
            {
                Debug.Log(sortedOres.Count);
                float t = 0f + l;
                time = 0f;
                getChar().addToInventory(CurrentBar.GetComponent<Item>());
                foreach (Item item in sortedOres)
                {
                    t += ((Ore)item).purity;
                    smelted.Add(item);
                    if(t >= 1f)
                    {
                        l = t - 1f;
                        break;
                    }
                }
                Debug.Log(sortedOres.Count);
                foreach (Item item in smelted)
                {
                    getChar().removeFromInventory(item);
                    sortedOres.Remove(item);
                }             
                current++;
                getCamera().smeltOne();
            }
            getCamera().updateFurnaceBar(time / delay);
            if(current >= max)
            {
                stopSmelting();
            }
        }
    }

    public void stopSmelting()
    {
        getCamera().stopSmelting();
        smelting = false;
    }
    
    public MetalBar getCurrentBar()
    {
        return CurrentBar.GetComponent<MetalBar>();
    }

    public int getCurrent()
    {
        return current;
    }
    public override GameObject getObject()
    {
        return null;
    }

    public override void set(Item i)
    {

    }
}
