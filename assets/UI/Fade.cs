using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {
    private float time = 0f;
    private float fade = 5f;
    private Color OG;
    private int iterations = 0, max = 60;
    private float maxAlpha;
    public bool full = false;
	// Update is called once per frame
    void Start()
    {
        OG = GetComponent<Image>().color;
        maxAlpha = GetComponent<Image>().color.a;
    }

    private bool faded = false;
    private bool delay = false;
    private float delayTime = 3f;
	void Update () {
        if(!faded)
        {
            time += Time.deltaTime;
            if (time >= fade)
            {
                if (!full)
                {
                    OG.a = Mathf.Lerp(0.3f, maxAlpha, (max - (float)iterations) / max);
                }
                else
                {
                    OG.a = Mathf.Lerp(0, maxAlpha, (max - (float)iterations) / max);
                }
                GetComponent<Image>().color = OG;
                iterations++;
                if(iterations==max)
                {
                    faded = true;
                }
            }
        }
        else if(delay)
        {
            time += Time.deltaTime;
            if(time >= delayTime)
            {
                faded = false;
                delay = false;
                time = 0;
                iterations = 0;
            }
        }
	}

    public void initDelay()
    {
        time = 0;
        delay = true;
        OG.a = maxAlpha;
        GetComponent<Image>().color = OG;
    }
}
