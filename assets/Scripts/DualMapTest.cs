using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualMapTest : MonoBehaviour {
    public MapGen MapA, MapB;
    private int mapSize = 32;
    private int chunkSize = 32;//max size for chunksize is 250, 251 is over the mesh vertex limit
    float[,] MapHeights;
    float chunkscale;
    public Material mat;
    public string Seed;
	// Use this for initialization
	void Start () {
        MapA = new MapGen();
        MapB = new MapGen();
        MapHeights = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        MapA.run(-1, -1, mapSize, chunkSize, mat);
        MapB.run(-1, -1, mapSize, chunkSize, mat);
        setSeeds();
        chunkscale = MapA.getChunkScale();
        Combine();
        load();
    }
    

    private void setSeeds()
    {
        Seed = MapA.getSeed() + ":" + MapB.getSeed();
    }
    
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
                MapHeights[x, y] = a  + b;
                if(MapHeights[x,y] < 2f)
                {
                    MapHeights[x, y] = -15;
                }
            }
        }
    }

    private int mapWH = 50000;

    Chunk[,] chunkMap;
	void load()
    {
        chunkMap = new Chunk[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, MapHeights);
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
                    chunkMap[x, y] = c;
                }
                else
                {
                    chunkMap[x, y] = null;
                }
            }
        }
    }


    public GameObject tree;
    public void loadTrees()
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
                    RaycastHit rayHit;

                    if (Physics.Raycast(temp, Vector3.down, out rayHit, 50000))
                    {
                        if (rayHit.point.y > 25)
                        {
                            GameObject t = GameObject.Instantiate(tree);
                            t.transform.position = rayHit.point;
                        }
                    }

                }
            }
        }
        
    }
}
