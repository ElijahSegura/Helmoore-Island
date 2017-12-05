using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerlinForms {
    List<Vector3> river = new List<Vector3>();
    List<List<Vector3>> splits = new List<List<Vector3>>();
    List<Vector3> points = new List<Vector3>();
    float widthVariation = 250f;
    float f = (80);
    // Use this for initialization
    private Vector3 s, e;





    //what i want, slope towards the lowest point
    public PerlinForms (Vector3 start, Vector3 end) {
        start.y = 0;
        end.y = 0;
        int split = Random.Range(0, 3);
        int nSplit = 0;
        float per = 0f;
        float xSlope = 1f, ySlope = 1f;
        this.s = start;
        this.e = end;
        int iter = 100;
        float rxoff = Random.Range(-1000000f, 1000000f), ryoff = Random.Range(-1000000f, 1000000f);
        for (int i = 0; i < iter; i++)
        {
            per = i / (float)iter;
            float width = widthVariation * (0.5f - Mathf.PerlinNoise((start.x + i + rxoff) / f, (start.z + i + ryoff) / f));
            Vector3 t = Vector3.Lerp(start, end, per);
            t.x -= width * xSlope;
            t.z += width * ySlope;
            river.Add(t);
            points.Add(t);
            if (per > 0.2f)
            {
                if (nSplit < split)
                {
                    List<Vector3> tempList = new List<Vector3>();
                    tempList.Add(t);
                    if (Random.Range(0f, 100f) < 20)
                    {
                        Vector3 tempend = new Vector3(t.x + ((end.x - t.x) * Random.Range(0f, 1f - per)), 0, t.z + ((end.z - t.z) * Random.Range(0f, 1f - per)));
                        Vector3 tempstart = new Vector3(t.x, 0, t.z);
                        float txs = 1f, tys = 1f;
                        for (int j = 0; j < iter; j++)
                        {
                            width = widthVariation * (0.5f - Mathf.PerlinNoise((tempstart.x + j + rxoff) / f, (tempstart.z + j + ryoff) / f));
                            per = j / (float)iter;
                            t = Vector3.Lerp(tempstart, tempend, per);
                            Vector3 xt = new Vector3(t.x, 0, t.z);
                            xt.x += width * txs;
                            xt.z += width * tys;
                            t = Vector3.Lerp(t, xt, 0.5f);
                            tempList.Add(t);
                            points.Add(t);
                        }
                        nSplit++;
                        splits.Add(tempList);
                    }

                }
            }
        }
        splits.Add(river);
    }

    public Vector3 getStart()
    {
        return s;
    }

    public Vector3 getEnd()
    {
        return e;
    }

    public List<Vector3> getPoints()
    {
        return points;
    }

    public List<List<Vector3>> getRivers()
    {
        return splits;
    }

    /*

    if(end.z > end.x)
        {
            xSlope = end.x / end.z;
        }
        else
        {
            ySlope = end.z / end.x;
        }

    if(tempend.x < 0 && tempend.z < 0)
                        {
                            if (tempend.z < tempend.x)
                            {
                                txs = tempend.x / tempend.z;
                            }
                            else
                            {
                                tys = tempend.z / tempend.x;
                            }
                        }
                        else if(tempend.x > 0 && tempend.z > 0)
                        {
                            if (tempend.z > tempend.x)
                            {
                                txs = tempend.x / tempend.z;
                            }
                            else
                            {
                                tys = tempend.z / tempend.x;
                            }
                        }


















        if(nOfSplits < splits)
            {
                if (Random.Range(0, 100) < 5)
                {
                    float rxoff = Random.Range(-100000, 100000), ryoff = Random.Range(-100000, 100000);
                    float percent = i / 100f;
                    float xper = 0f, yper = 0f;
                    if(Random.Range(0f, 1f) < 0.5f)
                    {
                        xper = Random.Range(percent, percent + 0.3f);
                        yper = Random.Range(percent, 1f);
                    }
                    else
                    {
                        yper = Random.Range(percent, percent + 0.3f);
                        xper = Random.Range(percent, 1f);
                    }
                    Vector3 tempend = new Vector3(end.x * xper, 0, end.z * yper);
                    Vector3 tempStart = new Vector3(point.transform.position.x, 0, point.transform.position.z);
                    int length = Random.Range(50, 80);
                    for (int j = 0; j < length; j++)
                    {
                        point.transform.position = Vector3.Lerp(tempStart, tempend, (j / (float)length));
                        float w = (widthVariation) * (0.5f - Mathf.PerlinNoise((point.transform.position.x + rxoff) / 20, (point.transform.position.z + ryoff) / 20));
                        if (w > 0)
                        {
                            point.transform.position = new Vector3(point.transform.position.x, 0, point.transform.position.z + w);
                        }
                        else
                        {
                            point.transform.position = new Vector3(point.transform.position.x - w, 0, point.transform.position.z);
                        }
                        river.Add(point.transform.position);
                    }
                    point.transform.position = tempStart;
                    nOfSplits++;

                }


            }




    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (List<Vector3> spl in splits)
        {
            Vector3[] temparray = spl.ToArray();
            for (int i = 0; i < temparray.Length; i++)
            {
                if (i + 1 < temparray.Length)
                {
                    Vector3 left = temparray[i];
                    Vector3 right = temparray[i + 1];
                    Gizmos.DrawLine(left, right);
                }
            }
        }
        
    }








    */
}
