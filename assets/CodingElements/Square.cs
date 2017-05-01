using UnityEngine;
using System.Collections;
using Delaunay.Geo;
using System.Collections.Generic;

public class Square {
    private bool enabled = true;
    private Vector3[] points;
    private Triangle t1, t2;
    private Polygon poly;
    private string biome;
    private bool snow = false;
    public Square(Vector3[] p, int[] i, float biome)
    {
        List<Vector2> ts = new List<Vector2>();
        foreach (Vector3 t in p)
        {
            ts.Add(new Vector2(t.x, t.z));
        }
        poly = new Polygon(ts);
        points = p;
        t1 = new Triangle(new Vector3[] {p[0], p[1], p[2] }, new int[] { i[0], i[1], i[2] });
        t2 = new Triangle(new Vector3[] { p[2], p[1], p[3] }, new int[] { i[2], i[1], i[3]});
        if (biome == 1f)
        {
            this.biome = "Grass";
        }
    }

    public Square(float biomeN)
    {
        if (biomeN == 1f)
        {
            this.biome = "Grass";
        }
                
    }

    public void setField(Vector3[] p, int[] i)
    {
        List<Vector2> ts = new List<Vector2>();
        foreach (Vector3 t in p)
        {
            ts.Add(new Vector2(t.x, t.z));
        }
        poly = new Polygon(ts);
        points = p;
        t1 = new Triangle(new Vector3[] { p[0], p[1], p[2] }, new int[] { i[0], i[1], i[2] });
        t2 = new Triangle(new Vector3[] { p[2], p[1], p[3] }, new int[] { i[2], i[1], i[3] });
    }

    public int[] getTriangles()
    {
        List<int> ts = new List<int>();
        foreach (int i in t1.getInts())
        {
            ts.Add(i);
        }
        foreach (int i2 in t2.getInts())
        {
            ts.Add(i2);
        }
        return ts.ToArray();
    }

    public Polygon getPoly()
    {
        return poly;
    }

    public bool pip(Vector3 P)
    {
        return poly.isInPolygon(P);
    }

    public void disable()
    {
        this.enabled = false;
    }


    public bool getEnabled()
    {
        return enabled;
    }

    public Vector3 getCenter()
    {
        float x = 0;
        float y = 0;
        foreach (Vector3 v in points)
        {
            x += v.x;
            y += v.z;
        }
        y /= points.Length;
        x /= points.Length;
        return new Vector3(x, 0, y);
    }

    public float[] getXLimits()
    {
        return new float[] { points[0].x, points[1].x };
    }
}
