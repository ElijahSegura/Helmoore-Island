using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilliageGen : MonoBehaviour {
    private int mapSize = 32;
    private int chunkSize = 32;//max size for chunksize is 250, 251 is over the mesh vertex limit
    float[,] MapHeights;
    float[,] MapHeights2;
    float chunkscale;
    public Material mat;
    public string Seed;
	// Use this for initialization
	void Start () {
        
        MapHeights = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        MapHeights2 = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        while(!(MapHeights[(chunkSize * mapSize) / 2, (chunkSize * mapSize) / 2] < 0.5 && MapHeights[(chunkSize * mapSize) / 2, (chunkSize * mapSize) / 2] > .48))
        {
            float offsetX = Random.Range(-500000f, 500000f);
            float offsety = Random.Range(-500000f, 500000f);
            for (int x = 0; x < chunkSize * mapSize; x++)
            {
                for (int y = 0; y < chunkSize * mapSize; y++)
                {
                    MapHeights[x, y] = Mathf.PerlinNoise((x + offsetX) / 100f, (y + offsety) / 100f);
                }
            }



            offsetX = Random.Range(-50000f, 50000f);
            offsety = Random.Range(-50000f, 50000f);
            for (int x = 0; x < chunkSize * mapSize; x++)
            {
                for (int y = 0; y < chunkSize * mapSize; y++)
                {
                    MapHeights2[x, y] = Mathf.PerlinNoise((y + offsetX) / 250f, (x + offsety) / 250f);
                }
            }
        }
        Combine();
        Modify();
        setTexture();
    }
    
    private void Combine()
    {
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                float a = MapHeights[x,y] / 2f;
                float b = MapHeights2[x,y] / 2f;

                MapHeights[x, y] = a + b;
                if(MapHeights[x,y] > 0.5f || (MapHeights[x, y] < 0.48f))
                {
                    MapHeights[x, y] = 0;
                }
            }
        }
    }

    private void Modify()
    {
        int center = chunkSize * mapSize;
        center /= 2;
        float size = (float)(center / 1.2);
        float maxDistance = Vector3.Distance(Vector3.zero, new Vector3(-size, 0, 0));
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                float posX = x - center, posY = y - center;
                MapHeights[x, y] = MapHeights[x, y] * (1 - (Vector3.Distance(Vector3.zero, new Vector3(posX, 0, posY)) / maxDistance));
                if(MapHeights[x,y] > 0)
                {
                    MapHeights[x, y] = 1;
                }
            }
        }
    }

    private void setTexture()
    {
        Texture2D p = new Texture2D(mapSize * chunkSize, mapSize * chunkSize);
        for (int x = 0; x < chunkSize * mapSize; x++)
        {
            for (int y = 0; y < chunkSize * mapSize; y++)
            {
                p.SetPixel(x, y, Color.white * MapHeights[x, y]);
            }
        }
        p.Apply();
        mat.mainTexture = p;
    }

    private int mapWH = 50000; 
}
