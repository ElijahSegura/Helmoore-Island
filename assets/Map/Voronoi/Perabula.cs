using System.Collections.Generic;
using UnityEngine;

internal class Perabula
{
    private Vector3 origin3D = Vector3.zero;
    public int iteration = 0;
    private float h, k;
    List<Vector3> verts = new List<Vector3>();
    private int length = 500;

    public Perabula(Vector2 origin)
    {
        origin3D.Set(origin.x, 0, origin.y);
        calculate();
    }

    public void draw()
    {
        for (int i = 0; i < verts.Count; i++)
        {
            if(i + 1 < verts.Count)
            {
                 Debug.DrawLine(verts.ToArray()[i], verts.ToArray()[i + 1], Color.red, 0.025F);
            }
        }
    }

    public void calculate()
    {
        iteration++;
        verts.Clear();
        for (int i = -length; i <= length; i++)
        {
            float y = origin3D.x - (Mathf.Pow(i, 2) / (iteration * 2));
            verts.Add(new Vector3(y + (iteration / 2), 0, origin3D.z + i));
        }
    }

    public Vector3 Origin()
    {
        return origin3D;
    }

    public int getIteration()
    {
        return iteration;
    }

}
