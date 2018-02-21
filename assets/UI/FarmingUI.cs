using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmingUI : MonoBehaviour, IDropHandler {
    private FarmPlot f;
    private Seed possible;
    private bool hasCrop = false;
    public int index;

    

    public void setFarm(FarmPlot farm)
    {
        this.f = farm;
        if(f.getPlot(index).doesHaveCrop())
        {
            hasCrop = true;
            transform.GetChild(1).GetComponent<Image>().color = Color.white;
            transform.GetChild(1).GetComponent<Image>().sprite = f.getPlot(index).icon;
        }
        else
        {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
        }
    }

    void OnDisable()
    {
        f = null;
    }

    public bool plotHasCrop()
    {
        return hasCrop;
    }

    public void setPossiblePlant(Seed s)
    {
        this.possible = s;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIInventoryItem seed = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        
        if(seed != null)
        {
            f.getPlot(index).setCrop(((Seed)seed.getItem()));
            transform.GetChild(1).GetComponent<Image>().sprite = ((Seed)seed.getItem()).icon;
            transform.GetChild(1).GetComponent<Image>().color = Color.white;
        }
    }
}
