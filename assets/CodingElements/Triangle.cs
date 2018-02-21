using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangle {
    private Vector3[] points = new Vector3[3];
    private int[] iPoint = new int[3];
    public Triangle(Vector3[] v, int[] i)
    {
        iPoint = i;
        points = v;
    }

    public Triangle(Vector3[] points)
    {

    }

    public int[] getInts()
    {
        return iPoint;
    }
}
