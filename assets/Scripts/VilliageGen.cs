using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VilliageGen : MonoBehaviour {
    private int width = 32;
    private int height = 32;
    public Material mat;
    float[,] MapHeights;
    float[,] area;
    private float scale = 200;
    private int midX, midY;
    private float offX, offY;
	// Use this for initialization
	void Start () {
        midX = (width * height) / 2;
        midY = (width * height) / 2;
        offX = Random.Range(0f, 500000f);
        offY = Random.Range(0f, 500000f);
        MapHeights = new float[width * height, width * height];
        area = new float[width * height, width * height];
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
        for (int x = 0; x < width * height; x++)
        {
            for (int y = 0; y < height * width; y++)
            {
                area[x, y] += Mathf.PerlinNoise((x + offX) / scale / 2, (y + offY) / scale / 2);
                if (area[x, y] < 0.3f)
                {
                    area[x, y] = 0f;
                }
                else
                {
                    area[x, y] = 1f;
                }
                MapHeights[x, y] *= area[x, y];
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
        int cy = 0;
        int cx = 0;
        bool gy = false;
        bool gx = false;
        for (int x = 0; x < width * height; x++)
        {
            cy = 0;
            cx = 0;
            gy = false;
            gx = false;
            for (int y = 0; y < width * height; y++)
            {


                if (!gy)
                {
                    if (MapHeights[x, y] == 1f)
                    {
                        if (y + 1 < width * height)
                        {
                            if (MapHeights[x, y + 1] == 0f)
                            {
                                gy = true;
                            }
                        }
                    }
                }
                else
                {
                    if (y + 1 < width * height)
                    {
                        if (MapHeights[x, y] == 0f)
                        {
                            cy++;
                        }
                        else if (MapHeights[x, y + 1] == 1f)
                        {
                            MapHeights[x, y - (cy / 2)] = 0.5f;
                            gy = false;
                            cy = 0;
                        }
                    }
                }

                if (!gx)
                {
                    if (MapHeights[y, x] == 1f)
                    {
                        if (y + 1 < width * height)
                        {
                            if (MapHeights[y + 1, x] == 0f)
                            {
                                gx = true;
                            }
                        }
                    }
                }
                else
                {
                    if (y + 1 < width * height)
                    {
                        if (MapHeights[y, x] == 0f)
                        {
                            cx++;
                        }
                        else if (MapHeights[y + 1, x] == 1f)
                        {
                            MapHeights[y - (cx / 2), x] = 0.5f;
                            gx = false;
                            cx = 0;
                        }
                    }
                }

            }


        }

        for (int x = 0; x < height * width; x++)
        {
            for (int y = 0; y < height * width; y++)
            {
                if(MapHeights[x,y] != 0.5f)
                {
                    MapHeights[x, y] = 0f;
                }
                else if(MapHeights[x,y] == 0.5f)
                {
                    MapHeights[x, y] = 1f;
                }
            }
        }
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
