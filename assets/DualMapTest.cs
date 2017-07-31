using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualMapTest : MonoBehaviour {
    public MapGen MapA, MapB;
    public int mapSize = 10;
    public int chunkSize = 10; //max size for chunksize is 250, 251 is over the mesh vertex limit
    public float[,] vertZ;
    public float chunkscale;
    public Material mat;
    public string Seed;
	// Use this for initialization
	public void Start () {
        Debug.Log("starting");
        HighPoly = new Square[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        LowPoly = new Square[((chunkSize + 1) / 4) * mapSize, ((chunkSize + 1) / 4) * mapSize];
        vertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        if (!string.IsNullOrEmpty(Seed))
        {
            string[] seeds = Seed.Split(':');
            string[] mapASeeds = seeds[0].Split(',');
            string[] mapBSeeds = seeds[1].Split(',');
            MapA.run(int.Parse(mapASeeds[0]), int.Parse(mapASeeds[1]));
            MapB.run(int.Parse(mapBSeeds[0]), int.Parse(mapBSeeds[1]));
        }
        else
        {
            MapA.run(-1, -1);
            MapB.run(-1, -1);
        }
        setSeeds();
        chunkscale = MapA.getChunkScale();
        Combine();
        genSquares();
        //load();
        //Destroy(MapA);
        //Destroy(MapB);
        //Destroy(gameObject);
    }

    private void setSeeds()
    {
        Seed = MapA.getSeed() + ":" + MapB.getSeed();
    }

    public Square[,] GetHighPolyMap()
    {
        return HighPoly;
    }

    public Square[,] GetLowPolyMap()
    {
        return HighPoly;
    }

    private Square[,] HighPoly;
    private Square[,] LowPoly;
    private void Combine()
    {
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                float a = MapA.getHeight(x, y);
                float b = (MapB.getHeight(x, y));
                if(a < 0)
                {
                    a = 0;
                }
                if(b < 0)
                {
                    b = 0;
                }
                vertZ[x, y] = a  + b;
                if(vertZ[x,y] < 2f)
                {
                    vertZ[x, y] = -15;
                }
            }
        }
    }

    private int mapWH = 50000;
    private void genSquares()
    {
        for (int a = 0; a < mapSize; a++)
        {
            for (int b = 0; b < mapSize; b++)
            {
                int iteration = 0;
                for (int x = 0; x <= chunkSize; x++)
                {
                    for (int y = 0; y <= chunkSize; y++)
                    {
                        int px = (chunkSize * a) + x;
                        int py = (chunkSize * b) + y;
                        HighPoly[px, py] = new Square(new Vector3[] { new Vector3(x * chunkscale - (mapWH / 2), 0, y * chunkscale - (mapWH / 2)), new Vector3(x * chunkscale - (mapWH / 2), 0, (y + 1) * chunkscale - (mapWH / 2)), new Vector3((x + 1) * chunkscale - (mapWH / 2), 0, (y + 1) * chunkscale - (mapWH / 2)), new Vector3((x + 1) * chunkscale - (mapWH / 2), 0, y * chunkscale - (mapWH / 2)) }, new int[] { iteration, iteration + 1, iteration + chunkSize + 1, iteration + chunkSize + 2 });
                        iteration++;
                    }
                }
            }
        }
        
    }

    Chunk[,] chunkMap;
	void load()
    {
        chunkMap = new Chunk[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, vertZ, HighPoly);
                if (!c.isflat())
                {
                    GameObject temp = new GameObject(x + ":" + y);
                    temp.AddComponent<MeshRenderer>();
                    temp.GetComponent<MeshRenderer>().receiveShadows = true;
                    temp.AddComponent<MeshFilter>();
                    temp.GetComponent<MeshFilter>().mesh = c.getMesh();
                    temp.AddComponent<MeshCollider>();
                    temp.GetComponent<MeshCollider>().sharedMesh = c.getMesh();
                    temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                    temp.GetComponent<MeshRenderer>().material = mat;
                    temp.transform.position = new Vector3(((x * (chunkSize * chunkscale)) - (50000 / 2)), 0, ((y * (chunkSize * chunkscale)) - (50000 / 2)));
                }
            }
        }
    }
}
