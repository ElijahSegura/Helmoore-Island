using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetResolutions : MonoBehaviour {

    // Use this for initialization
    List<Resolution> temp = new List<Resolution>();
    List<Dropdown.OptionData> options;
    void Start () {
        getSupported(null);  
    }

    public void getSupported(GameObject aspect)
    {
        temp.Clear();
        if(options != null)
        {
            options.Clear();
        }
        float width, height;
        if(aspect != null)
        {
            Dropdown.OptionData o = aspect.GetComponent<Dropdown>().options[aspect.GetComponent<Dropdown>().value];
            width = float.Parse(o.text.Split(':')[0]);
            height = float.Parse(o.text.Split(':')[1]);
        }
        else
        {
            width = 16;
            height = 9;
        }
        //4:3 - 1.3333
        //5:4 - 1.25
        //16:10 - 1.6
        //16:9 - 1.777778
        Resolution[] resolutions = Screen.resolutions;
        options = GetComponent<Dropdown>().options;
        foreach (Resolution r in resolutions)
        {
            if (!temp.Contains(r) && ((float)r.width / r.height) == (width / height))
            {
                temp.Add(r);
            }
        }
        changeResolutions();
    }
    
    private void changeResolutions()
    {
        GetComponent<Dropdown>().options.Clear();
        foreach (Resolution r in temp)
        {
            options.Add(new Dropdown.OptionData(r.width + "x" + r.height + " " + (r.refreshRate) + "Hz"));
        }
        GetComponent<Dropdown>().value = options.Count;
    }
}
