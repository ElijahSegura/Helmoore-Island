using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTip : MonoBehaviour {
    public GameObject top, bot;
    public bool isTop = false;
	void Update () {
		if(isTop)
        {
            transform.position = top.transform.position;
        }
        else
        {
            transform.position = bot.transform.position;
        }
	}
}
