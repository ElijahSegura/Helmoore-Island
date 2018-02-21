using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Vector3 open = new Vector3(0,120,0);
    private Vector3 close = Vector3.zero;
    private Vector3 perFrame;
    private int openit = 0;
    private int iterations = 60;

    void Start()
    {
        perFrame = Vector3.Lerp(close, open, (float)1 / iterations);
    }


    public void Open(Vector3 hit)
    {
        if (!opening && !closing)
        {
            if (closed)
            {
                float dir = (transform.position - hit).z;
                
                if (dir > 0)
                {
                    open.y = 120f;
                    perFrame = Vector3.Lerp(close, open, (float)1 / iterations);
                    opening = true;
                }
                else if (dir < 0)
                {
                    open.y = -120f;
                    perFrame = Vector3.Lerp(close, open, (float)1 / iterations);
                    opening = true;
                }
            }
            else if (opened)
            {
                closing = true;
            }
        }
    }

    private bool opening = false;
    private bool closing = false;
    private bool opened = false, closed = true;
    void Update()
    {
        if(opening)
        {
            if(openit < iterations)
            {
                openit++;
                transform.Rotate(perFrame);
            }
            else if(openit >= iterations)
            {
                opening = false;
                closing = false;
                opened = true;
                closed = false;
                openit = 0;
            }
        } 
        else if(closing)
        {
            if (openit < iterations)
            {
                openit++;
                transform.Rotate(-perFrame);
            }
            else if (openit >= iterations)
            {
                openit = 0;
                opening = false;
                closing = false;
                closed = true;
                opened = false;
            }
        }
    }
}
