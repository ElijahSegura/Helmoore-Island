using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilliageGen : MonoBehaviour {
    private int width = 32;
    private int height = 32;
    public Material mat;
    float[,] MapHeights;
    private float scale = 90;
    private int midX, midY;
    private float offX, offY;
    private float dist;
	// Use this for initialization
	void Start () {
        midX = (width * height) / 2;
        midY = (width * height) / 2;
        offX = Random.Range(0f, 500000f);
        offY = Random.Range(0f, 500000f);
        MapHeights = new float[width * height, width * height];
        for (int x = 0; x < width * height; x++)
        {
            for (int y = 0; y < height * width; y++)
            {
                MapHeights[x, y] += Mathf.PerlinNoise((x + offX) / scale, (y + offY) / scale);
                if(MapHeights[x,y] < 0.5f)
                {
                    MapHeights[x, y] = 0f;
                }
                else
                {
                    MapHeights[x, y] = 1f;
                }
            }
        }
        dist = 512f;
        for (int x = 0; x < width * height; x++)
        {
            for (int y = 0; y < height * width; y++)
            {
                if(Vector3.Distance(new Vector3(x - (512), 0, y - (512)), new Vector3(width / 2, height / 2)) > dist)
                {
                    MapHeights[x, y] = 0;
                }
            }
        }

        makePaths();
        setTexture();
    }

    private int path = 0;
    private int limit = 1000;
    private float low = 1f;
    private Vector2 pos;
    private string dir;
    private float n, s, e, w;
    private float current;
    private float newLow;
    private void makePaths()
    {
        
    }

    private void setTexture()
    {
        Texture2D p = new Texture2D(width * height, width * height);
        for (int x = 0; x < height * width; x++)
        {
            for (int y = 0; y < height * width; y++)
            {
                p.SetPixel(x, y, Color.white * MapHeights[x, y]);
            }
        }
        p.Apply();
        mat.mainTexture = p;
    }
}
