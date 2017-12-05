using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPlot : MonoBehaviour {
    public CropPlot getPlot(int index)
    {
        foreach (CropPlot c in GetComponentsInChildren<CropPlot>())
        {
            if(c.index == index) {
                return c;
            }
        }
        return null;
    }

    void Update()
    {
        foreach (CropPlot c in GetComponentsInChildren<CropPlot>())
        {
            if(c.doesHaveCrop()) {
                c.grow();
            }
        }
    }
}
