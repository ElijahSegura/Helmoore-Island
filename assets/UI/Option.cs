using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour {

    public GameObject resolusion;

	public void apply()
    {
        int i = resolusion.GetComponent<Dropdown>().value;
        Dropdown.OptionData res = resolusion.GetComponent<Dropdown>().options[i];
        string[] newScreen = res.text.Split(' ')[0].Split('x');
        int height = int.Parse(newScreen[1]);
        int width = int.Parse(newScreen[0]);
        Screen.SetResolution(width, height, true);
    }
}
