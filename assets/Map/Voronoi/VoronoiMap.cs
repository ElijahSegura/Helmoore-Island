using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiMap : MonoBehaviour {

    public GameObject dots;
    public GameObject leastOBJ;
    public int dotValue;
    private int gridSize;
    private int mms = 200000;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector3> tCenters = new List<Vector3>();
    public int mapSize = 3;
    public int chunkSize = 250;
    private int chunkScale;
    private Chunk[,] map;
    private List<Perabula> perabulas = new List<Perabula>();
	void Start () {
        Vector3 start = Vector3.zero;
        Vector3 end = new Vector3(100, 0, 100);
        Debug.DrawLine(start, end, Color.red, 5000);

        chunkScale = (mms / 3) / 250;
        Debug.Log(chunkScale);
        gridSize = (mms / dotValue) - 10;
        for (int x = 0; x < dotValue; x++)
        {
            for (int y = 0; y < dotValue; y++)
            {
                float offx = Random.Range((gridSize / 2) * -1, (gridSize / 2));
                float offy = Random.Range((gridSize / 2) * -1, (gridSize / 2));
                verts.Add(new Vector3(((x * gridSize) - ((dotValue * gridSize) / 2)) + offx, (y * gridSize - ((dotValue * gridSize) / 2)) + offy, 0));
            }
        }

        Vector3[] temparray = verts.ToArray(); 


        int iteration = 0;
        for (int i = 0; i <= ((dotValue + 1) * (dotValue + 1)) * 3; i += 3)
        {
            if ((1 + iteration) % (dotValue) != 0)
            {
                if ((dotValue + 1) + iteration + 1 < verts.Count)
                {
                    Vector3 temp = solveCenter(new Vector3[] { temparray[0 + iteration], temparray[1 + iteration], temparray[(dotValue + 1) + iteration] });
                    temp.z = temp.y;
                    temp.y = 0;
                    tCenters.Add(temp);
                    temp = solveCenter(new Vector3[] { temparray[(dotValue + 1) + iteration], temparray[1 + iteration], temparray[(dotValue + 1) + iteration + 1] });
                    temp.z = temp.y;
                    temp.y = 0;
                    tCenters.Add(temp);
                    iteration++;
                }
            }
            else
            {
                iteration++;
            }
        }

        temparray = tCenters.ToArray();
        Vector3 least;
        least = tCenters.ToArray()[0];
        for (int c = 0; c < tCenters.Count; c++)
        {
            if (temparray[c].x < least.x)
            {
                least = temparray[c];
            }
        }
        GameObject l = GameObject.Instantiate(leastOBJ);
        l.transform.position = least;
        Debug.Log(tCenters.Count);
    }

    int pin = -100000;
    void Update()
    {
        Debug.DrawLine(new Vector3(pin, 0, -100000), new Vector3(pin, 0, 100000), Color.blue, 0.025F);
        foreach (Vector3 v in tCenters)
        {
            if (v.x == pin)
            {
                perabulas.Add(new Perabula(new Vector2(v.x, v.z)));
                tCenters.Remove(v);
                break;
            }
        }
        pin++;
        foreach(Perabula c in perabulas)
        {
            c.calculate();
            c.draw();
        }
    }






    private Vector3 solveCenter(Vector3[] _vertices)
    {
        float x = 0;
        float y = 0;
        foreach (Vector2 v in _vertices)
        {
            x += v.x;
            y += v.y;
        }
        y /= _vertices.Length;
        x /= _vertices.Length;
        return new Vector3(x, 0, y);
    }
}
