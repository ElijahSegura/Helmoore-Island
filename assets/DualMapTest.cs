using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualMapTest : MonoBehaviour {
    public MapGen A, B;
    private int mapSize = 32;
    private int chunkSize = 32;//max size for chunksize is 250, 251 is over the mesh vertex limit
    float[,] vertZ;
    float chunkscale;
    public Material mat;
    public string Seed;
	// Use this for initialization
	void Start () {
        A = new MapGen();
        B = new MapGen();
        HighPoly = new Square[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        LowPoly = new Square[((chunkSize + 1) / 4) * mapSize, ((chunkSize + 1) / 4) * mapSize];
        vertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        A.run(-1, -1, mapSize, chunkSize, mat);
        B.run(-1, -1, mapSize, chunkSize, mat);
        setSeeds();
        chunkscale = A.getChunkScale();
        Combine();
        genSquares();
        load();
    }
    

    private void setSeeds()
    {
        Seed = A.getSeed() + ":" + B.getSeed();
    }
    
    private Square[,] HighPoly;
    private Square[,] LowPoly;
    private void Combine()
    {
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                float a = A.getHeight(x, y);
                float b = (B.getHeight(x, y));
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


    public GameObject tree;
    void Update()
    {
        Debug.Log("loading");
        int treeCount = 500;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                for (int i = 0; i < treeCount; i++)
                {
                    Vector3 temp = new Vector3(((x * (chunkSize * chunkscale)) - (50000 / 2)), 10000, ((y * (chunkSize * chunkscale)) - (50000 / 2)));
                    float px = Random.Range(-(chunkscale / 2f) * chunkSize, (chunkscale / 2f) * chunkSize);
                    float py = Random.Range(-(chunkscale / 2f) * chunkSize, (chunkscale / 2f) * chunkSize);
                    temp.x += px;
                    temp.z += py;
                    RaycastHit o;

                    if (Physics.Raycast(temp, Vector3.down, out o, 50000))
                    {
                        if (o.point.y > 25)
                        {
                            GameObject t = GameObject.Instantiate(tree);
                            t.transform.position = o.point;
                        }
                    }

                }
            }
        }
        
    }
}
