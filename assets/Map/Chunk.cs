using UnityEngine;
using System.Collections.Generic;
using Delaunay.Geo;

public class Chunk {
    private float[,] vertZ;

    private float scaleX, scaleY;
    private int chunkSize;
    private float chunkScale;
    private float maxMapHeight = 100;
    /*
    private float scale = 320.2F;
    private float heightScale = 1F;
    private float maxHeight = 300F;

    private float secondaryScale = 200.2F;
    private float secondaryHeightScale = 2F;
    private float secondaryMaxHeight = 50F;


    //tertiary
    private float ts = 240F;
    private float ths = 2F;
    private float tmh = 3F;

    //quadroary?
    private float fs = 80F;
    private float fhs = 2F;
    private float fmh = 14;

    //the water scale
    private float ws = 500F;
    private float whs = 2.5F;
    private float wmh = 2;
    */
    private Vector2 boundryCenter = Vector2.zero;
    private static List<Vector3> verts;
    private static List<Vector2> UVs;
    //private static List<GameObject> stuff = new List<GameObject>();
    private static List<int> tris;

    /*
    private void ifLoad(int chunkSize, int posx, int posy, int xOffset, int yOffset, List<Polygon> map, int x, int y, float scale)
    {
        int partx = chunkSize * posx;
        int party = chunkSize * posy;
        vertZ[x, y] = Mathf.PerlinNoise((x + xOffset) / scale, (y + yOffset) / scale);

        float z = vertZ[x, y];
        float X = (x * 2) - (chunkSize);
        float Y = (y * 2) - (chunkSize);
        verts.Add(new Vector3(X, z, Y));
        UVs.Add(new Vector2((x / (float)chunkSize), (y / (float)chunkSize)));
    }
    */
        
    public void setVert(int x, int y, float z)
    {
        try
        {
            vertZ[x, y] = z;
        }
        catch (System.Exception)
        {
            Debug.Log(x);
            Debug.Log(y);
            throw;
        }
        
    }

    



    public Chunk(int chunkSize, int posx, int posy, float chunkScale, float[,] vertZ, float maxHeight)
    {
        verts = new List<Vector3>();
        UVs = new List<Vector2>();
        tris = new List<int>();
        this.chunkSize = chunkSize;
        this.chunkScale = chunkScale;
        this.vertZ = vertZ;
        verts.Clear();
        tris.Clear();
        UVs.Clear();

        for (int x = 0; x <= chunkSize; x++)
        {
            for (int y = 0; y <= chunkSize; y++)
            {
                verts.Add(new Vector3(x * chunkScale, (maxMapHeight * (vertZ[(chunkSize * posx) + x, (chunkSize * posy) + y] / maxHeight)), y * chunkScale));
                //Debug.Log(new Vector3((((scale * posx) * chunkSize) - (mapSize / 2)) + (x * scale), 0, ((((scale * posy) * chunkSize)) - (mapSize / 2)) + (y * scale)));
                UVs.Add(new Vector2((x / (float)chunkSize), (y / (float)chunkSize)));

            }
        }

        int iteration = 0;
        for (int i = 0; i <= ((chunkSize + 1) * (chunkSize + 1)) * 3; i += 3)
        {
            if ((1 + iteration) % (chunkSize + 1) != 0)
            {
                if ((chunkSize + 1) + iteration + 1 <= verts.Count)
                {
                    tris.Add(0 + iteration);
                    tris.Add(1 + iteration);
                    tris.Add((chunkSize + 1) + iteration);
                    tris.Add((chunkSize + 1) + iteration);
                    tris.Add(1 + iteration);
                    tris.Add((chunkSize + 1) + iteration + 1);
                    iteration++;
                }
            }
            else
            {
                iteration++;
            }
        }
    }

    
    public Mesh getMesh()
    {
        
        
        

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.Optimize();
        mesh.SetUVs(0, UVs);
        return mesh;
    }
    
    public float leastDistanceToBoundry(Vector3 A ,List<Vector2> bounds, float greatest)
    {
        //get 2 vectors as least, in between the original vert(doesn't have to be intersecting) and then get distance from halfway point to original
        Vector2[] temparray = bounds.ToArray();
        Vector2 l = temparray[0];
        float least = Vector2.Distance(new Vector2(A.x, A.z), temparray[0]);
        for (int i = 0; i < temparray.Length; i++)
        {
            if (Vector2.Distance(new Vector2(A.x, A.z), temparray[i]) < least)
            {
                least = Vector2.Distance(new Vector2(A.x, A.z), temparray[i]);
            }
        }
        return least / greatest;
    }
}
/*

    bool inPoly = false;
                if (Vector2.Distance(new Vector2(0, 0), new Vector2(x + (chunkSize * posx), y)) < 50)
                {
                    foreach (Polygon p in map)
                    {
                    
                        if (p.isInPolygon(new Vector2(x, y)))
                        {
                            ifLoad(chunkSize, posx, posy, xOffset, yOffset, map, x, y, scale);
                            inPoly = true;
                        }
                    }
                }
                if(!inPoly)
                {
















    int inPoly = 0;
        for (int x = 0; x <= chunkSize; x++)
        {
            for (int y = 0; y <= chunkSize; y++)
            {
                bool added = false;
                Vector3 temp = new Vector3((((chunkScale * posx) * chunkSize) - (mapSize / 2)) + (x * chunkScale), 0, ((((chunkScale * posy) * chunkSize)) - (mapSize / 2)) + (y * chunkScale));

                foreach (Polygon p in map)
                {
                    if(p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp))
                        {
                            //if in polygon add to 1 in iteration and that is the height
                            if (!added)
                            {
                                inPoly++;
                                //float least = leastDistanceToBoundry(temp, Boundry, greatestLeast);
                                float height = maxHeight * (inPoly / 300);
                                //(Mathf.PerlinNoise((temp.x + xOffset) / perlinScale,(temp.z + yOffset) / perlinScale))
                                // polys.Add(new Vector3(x * chunkScale, 0, y * chunkScale), inPoly);

                                vertZ[x, y] = inPoly;


                                //verts.Add(new Vector3(x * chunkScale, inPoly, y * chunkScale));

                                //Debug.Log(new Vector3((((scale * posx) * chunkSize) - (mapSize / 2)) + (x * scale), 0, ((((scale * posy) * chunkSize)) - (mapSize / 2)) + (y * scale)));
                                //uvssss.Add(new Vector3(x * chunkScale, 0, y * chunkScale), new Vector2((x / (float)chunkSize), (y / (float)chunkSize)));
                                //UVs.Add(new Vector2((x / (float)chunkSize), (y / (float)chunkSize)));
                                added = true;
                            }
                        }
                    }
                    
                }
                if(!added)
                {
                    for (int i = 0; i < inPoly; i++)
                    {
                        if(y - inPoly <= 0 && x > 0 && chunkSize - i >= 0)
                        {
                            if (i < inPoly / 2)
                            {
                                vertZ[x - 1, chunkSize - i] = i;
                            }
                            else
                            {
                                vertZ[x - 1, chunkSize - i] = inPoly - i;
                            }
                        }
                        else
                        {
                            if (i < inPoly / 2)
                            {
                                vertZ[x, y - i] = i;
                            }
                            else
                            {
                                vertZ[x, y - i] = inPoly - i;
                            }
                        }
                        
                    }
                    inPoly = 0;
                    vertZ[x, y] = 0;
                    //verts.Add(new Vector3(x * chunkScale, 0, y * chunkScale));
                    //Debug.Log(new Vector3((((scale * posx) * chunkSize) - (mapSize / 2)) + (x * scale), 0, ((((scale * posy) * chunkSize)) - (mapSize / 2)) + (y * scale)));
                    //UVs.Add(new Vector2((x / (float)chunkSize), (y / (float)chunkSize)));
                }
                
            }
        }


    */
