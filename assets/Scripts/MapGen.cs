using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using System.Linq;
using System.Runtime.InteropServices;



public class MapGen
{
    private int VoronoiPointCount = 380;
    public List<Material> Materials = new List<Material>();
    private Map serverMap;//THIS IS WHAT WE WILL SEND OVER THE SERVER TO THE CLIENT, IT WILL HAVE MAPHEIGHT DATA, CURRENT CHUNK DATA, AND MAYBE SOME OTHER THINGS then we'll destroy the object that has this script to clean up some memory from the 2d arrays
    //blue is ocean, yellow is desert, green is plains, dark green is forest, purple is swamp
    private Color[] biomes = new Color[] { Color.blue, Color.yellow, Color.green, new Color(Color.green.r / 2, Color.green.g / 2, Color.green.b / 2), Color.red + Color.blue, Color.white };
    private Chunk[,] chunkMap;
    private int mapSize;
    private int chunkSize; //max size for chunksize is 250, 251 is over the mesh vertex limit
    private float Iterator;
    private float chunkscale;
    private int rivers;
    private Vector2 proxyEquator;
    private int[,] biome;
    private List<Vector2> boundary = new List<Vector2>();
    private List<PerlinForms> riverForms = new List<PerlinForms>();
    private List<Vector2> tempRiver = new List<Vector2>();
    private List<Polygon> Map = new List<Polygon>();
    private List<Polygon> VoronoiMap = new List<Polygon>();
    private List<Vector2> m_points;
    private float mapWH = 50000;
    private float scale = 4.0F;
    private int perlinNoiseXOffset, perlinNoiseYOffset;
    private float biomeDistance = 0f;

    private List<Vector2> riverEnds = new List<Vector2>();
    private List<Vector2> secondaryPoints = new List<Vector2>();
    private List<Polygon> secondPolygons = new List<Polygon>();
    private List<Polygon> finalVoronoiMap = new List<Polygon>();

    private float maxHeight = 4096f;
    
    private int[,] islands;
    private float[,] mapHeights;
    private Square[,] Squares;
    private float[,] secondairyHeights;
    private float[,] PerlinNoise;
    public void run(int seedx, int seedy, int maps, int chunks, Material mat)
    {
        mapSize = maps;
        chunkSize = chunks;
        Materials.Add(mat);
        if (seedx >= 0 && seedy >= 0)
        {
            perlinNoiseXOffset = seedx;
            perlinNoiseYOffset = seedy;
        }
        else
        {
            perlinNoiseXOffset = Random.Range(0, 2000000);
            perlinNoiseYOffset = Random.Range(0, 2000000);
        }
        PerlinNoise = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        mapV v = new mapV();
        
        proxyEquator = new Vector2(Random.Range(-(mapWH / 3), mapWH / 3), Random.Range(-(mapWH / 3), mapWH / 3));
        serverMap = new Map((chunkSize + 1) * mapSize);
        
        rivers = Random.Range(20, 40);
        Iterator = 1;
        chunkscale = (mapWH / mapSize) / (chunkSize);
        genPTex();
        gen();
        load();
    }

    public string getSeed()
    {
        return perlinNoiseXOffset + "," + perlinNoiseYOffset;
    }

    private void gen()
    {
        List<uint> colors = new List<uint>();
        List<uint> colors2 = new List<uint>();
        List<uint> colors3 = new List<uint>();
        m_points = new List<Vector2>();

        for (int i = 0; i < VoronoiPointCount; i++)
        {
            colors.Add(0);
            m_points.Add(new Vector2(
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2),
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2))
            );
        }

        for (int i = 0; i < 750; i++)
        {
            colors2.Add(0);
            secondaryPoints.Add(new Vector2(
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2),
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2))
            );
        }


        



        Voronoi voronoiMapA = new Voronoi(m_points, colors, new Rect(-mapWH / 2, -mapWH / 2, mapWH, mapWH));
        Voronoi voronoiMapB = new Voronoi(secondaryPoints, colors2, new Rect(-mapWH / 2, -mapWH / 2, mapWH, mapWH));


        foreach (List<Vector2> l in voronoiMapA.Regions())
        {
            Polygon p = new Polygon(l);
            foreach (Vector2 vec in p.points())
            {
                Vector2 xy = getXYFromPos(new Vector3(vec.x, 0, vec.y));
                if(PerlinNoise[(int)xy.x,(int)xy.y] > 0.2)
                {
                    if(Vector3.Distance(Vector3.zero, new Vector3(vec.x, 0, vec.y)) < (mapWH / 2) * 0.78f)
                    {
                        Map.Add(p);
                        break;
                    }
                }
            }
        }

        Debug.Log(Map.Count);

        foreach (Polygon p in Map)
        {
            foreach (Polygon p2 in Map)
            {
                if (!p.Equals(p2))
                {
                    p.testConnected(p2);
                }
            }
            if (p.getConnections() > 0 && p.getIsland().ToArray()[0].getConnections() > 1)
            {
                VoronoiMap.Add(p);
            }
        }





        foreach (Polygon p in VoronoiMap)
        {
            foreach (List<Vector2> l in voronoiMapB.Regions())
            {
                Polygon p2 = new Polygon(l);
                bool b = false;
                if(p.isInPolygon(p2.getCenter(), mapWH))
                {
                    finalVoronoiMap.Add(p2);
                }
                else
                {
                    foreach (Vector2 VP in p2.points())
                    {
                        Vector3 tempV = new Vector3(VP.x, 0, VP.y);
                        if (!b)
                        {
                            if (p.isAroundPolygon(tempV))
                            {
                                if (p.isInPolygon(tempV, mapWH))
                                {
                                    finalVoronoiMap.Add(p2);
                                    b = true;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
    List<Vector2> points = new List<Vector2>();
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>


    private void load()
    {
        biome = new int[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        mapHeights = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        Squares = new Square[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        secondairyHeights = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        islands = new int[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        genHeights(finalVoronoiMap);
        smooth(5, mapHeights);
        smooth(120, VoronoiMapCusomNoise);
        modifyMesh();
        //
        genMoisture();
        moistureTex();
        detectIslands();
        //genRivers();
        //genBiomes();

        genSquares();
        //genTexture();
        //genHeightTex();
        //loadChunks();
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public float getChunkScale()
    {
        return chunkscale;
    }
    

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public Square[,] getSquares()
    {
        return Squares;
    }

    void genBiomes()
    {

        int distanceModifier = (int)(8000 / chunkscale);
        //foreach in riverend, foreach river point, divide by mapsize * chunksize and round up + -(mapsize * chunksize) to offset the distance from 0.
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (mapHeights[x, y] > 0 && biome[x, y] == 0)
                {
                    biome[x, y] = 1;
                }
            }
        }

        foreach (PerlinForms form in riverForms)
        {
            int isl = islands[(int)(Mathf.Ceil((form.getStart().x + (mapWH / 2)) / (chunkscale))), (int)(Mathf.Ceil((form.getStart().z + (mapWH / 2)) / (chunkscale)))];
            foreach (List<Vector3> River in form.getRivers())
            {
                foreach (Vector3 v in River)
                {
                    int x = (int)(Mathf.Ceil((v.x + (mapWH / 2)) / (chunkscale)));
                    int y = (int)(Mathf.Ceil((v.z + (mapWH / 2)) / (chunkscale)));
                    for (int i = 0; i < distanceModifier; i++)
                    {
                        for (int j = 0; j < distanceModifier; j++)
                        {
                            if ((x + i) - (distanceModifier / 2) < chunkSize * mapSize && (x + i) - (distanceModifier / 2) >= 0 && (y + j) - (distanceModifier / 2) < chunkSize * mapSize && (y + j) - (distanceModifier / 2) >= 0)
                            {
                                if (mapHeights[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] >= -4 && biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] != 8)
                                {
                                    Vector3 temp = new Vector3((((x + i) - (distanceModifier / 2)) * chunkscale - (mapWH / 2)), 0, (((y + j) - (distanceModifier / 2)) * chunkscale - (mapWH / 2)));
                                    if (Vector3.Distance(v, temp) < Random.Range(750f, 1000f))
                                    {
                                        if (islands[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] == isl)
                                        {
                                            biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] = 3;
                                        }
                                    }
                                    else if (Vector3.Distance(v, temp) < Random.Range(2500f, 3500f))
                                    {
                                        if (biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] != 3)
                                        {
                                            if (islands[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] == isl)
                                            {
                                                biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] = 2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    biome[x, y] = 8;
                }
            }
        }
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (biome[x, y] == 8)
                {
                    biome[x, y] = 0;
                }
                if (mapHeights[x, y] > 0)
                {
                    Vector3 temp = new Vector3(((x) * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
                    if (Vector3.Distance(temp, new Vector3(proxyEquator.x, 0, proxyEquator.y)) > biomeDistance)
                    {
                        biomeDistance = Vector3.Distance(temp, new Vector3(proxyEquator.x, 0, proxyEquator.y));
                    }
                }

            }
        }


        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            bool start = false;
            bool start2 = false;
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (!start)
                {
                    if (mapHeights[x, y] > 0)
                    {
                        biome[x, y - 1] = 1;
                        biome[x, y - 2] = 1;
                        biome[x, y] = 1;
                        start = true;
                    }
                }
                else
                {
                    if (mapHeights[x, y] < 0)
                    {
                        biome[x, y - 1] = 1;
                        biome[x, y] = 1;
                        start = false;
                    }
                }



                if (!start2)
                {
                    if (mapHeights[y, x] > 0)
                    {
                        biome[y, x - 1] = 1;
                        biome[y, x - 2] = 1;
                        biome[y, x] = 1;
                        start2 = true;
                    }
                }
                else
                {
                    if (mapHeights[y, x] < 0)
                    {
                        biome[y, x - 1] = 1;
                        biome[y, x + 1] = 1;
                        biome[y, x] = 1;
                        start2 = false;
                    }
                }
            }
        }
    }


    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    //Dis shit needs to be optimized BAD
    void genRivers()
    {
        float maxHeight = 0;
        Vector2 closestPoint = Vector2.zero;
        Vector2 highestPoint = new Vector2();
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (mapHeights[x, y] < 1 && mapHeights[x, y] > -5)
                {
                    boundary.Add(new Vector2((x * chunkscale - (mapWH / 2)), (y * chunkscale - (mapWH / 2))));
                }
                if (mapHeights[x, y] > maxHeight)
                {
                    maxHeight = mapHeights[x, y];
                    highestPoint = new Vector2((x * chunkscale - (mapWH / 2)), (y * chunkscale - (mapWH / 2)));
                }
            }
        }
        int ty = 0;
        float least = 100000;
        foreach (Vector2 item in boundary)
        {
            if (Vector2.Distance(highestPoint, item) < least)
            {
                least = Vector2.Distance(highestPoint, new Vector2(item.x, item.y));
                closestPoint = item;
            }
        }
        points.Add(highestPoint);



        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            maxHeight = 0;
            bool b = false;
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (mapHeights[x, y] > 0)
                {
                    if (mapHeights[x, y] > maxHeight)
                    {
                        maxHeight = mapHeights[x, y];
                        ty = y;
                        b = true;
                    }
                }
            }
            if (b)
            {
                points.Add(new Vector2((x * chunkscale - (mapWH / 2)), (ty * chunkscale - (mapWH / 2))));
            }
        }
        Vector2[] tempArray = new Vector2[points.Count];

        for (int i = 0; i < tempArray.Length; i++)
        {
            int s = Random.Range(0, points.Count - 1);
            tempArray[i] = points[s];
            points.Remove(points[s]);
        }
        points.Clear();
        points = new List<Vector2>(tempArray);

        int r = 0;
        while (r < rivers)
        {
            int s = Random.Range(0, points.Count - 1);
            Vector2 temp = points[s];
            if (riverEnds.Count == 0)
            {
                riverEnds.Add(points[s]);
                points.Remove(points[s]);
                r++;
            }
            else
            {
                bool b = true;
                foreach (Vector2 item in riverEnds)
                {
                    if (Vector2.Distance(item, temp) < 500)
                    {
                        b = false;
                    }
                }
                if (b)
                {
                    riverEnds.Add(points[s]);
                    points.Remove(points[s]);
                    r++;
                }
            }
        }



        foreach (Vector2 v in riverEnds)
        {
            List<Vector2> closest10 = new List<Vector2>();
            least = 200000;
            Vector3 l = Vector2.zero;
            foreach (Vector2 item in boundary)
            {
                //add boundry to closest10 and select a random one
                if (Vector2.Distance(v, item) < least)
                {
                    least = Vector2.Distance(v, item);
                    l = item;
                    closest10.Add(item);
                    if (closest10.Count >= 10)
                    {
                        closest10.RemoveAt(0);
                    }
                }
            }
            int ra = Random.Range(0, closest10.Count - 1);
            Vector3 temp = new Vector3(closest10[ra].x, 0, closest10[ra].y);
            PerlinForms p = new PerlinForms(new Vector3(v.x, 0, v.y), temp);
            riverForms.Add(p);
        }

    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    void genSquares()
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
                        Squares[px, py] = new Square(new Vector3[] { new Vector3(x * chunkscale - (mapWH / 2), 0, y * chunkscale - (mapWH / 2)), new Vector3(x * chunkscale - (mapWH / 2), 0, (y + 1) * chunkscale - (mapWH / 2)), new Vector3((x + 1) * chunkscale - (mapWH / 2), 0, (y + 1) * chunkscale - (mapWH / 2)), new Vector3((x + 1) * chunkscale - (mapWH / 2), 0, y * chunkscale - (mapWH / 2)) }, new int[] { iteration, iteration + 1, iteration + chunkSize + 1, iteration + chunkSize + 2 }, biome[x, y]);
                        iteration++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }

    int island = 0;
    private void FloodFillIsland(int x, int y)
    {
        Stack<Vector2> points = new Stack<Vector2>();
        points.Push(new Vector2(x, y));
        //fill left and right, if you can go up or down from original point add to points, then repeat with points
        //add points from all the way left
        while (points.Count > 0)
        {
            Vector2 origin = points.Pop();
            bool left = true, right = true;
            bool newUp = false;
            bool newDown = false;
            int Xorigin = (int)origin.x;
            int Yorigin = (int)origin.y;
            int l = 0, r = 0;

            if (mapHeights[Xorigin, Yorigin + 1] > 0 && islands[Xorigin, Yorigin + 1] == 0)
            {
                points.Push(new Vector2(Xorigin, Yorigin + 1));
            }
            else
            {
                newUp = true;
            }
            if (mapHeights[Xorigin, Yorigin - 1] > 0 && islands[Xorigin, Yorigin - 1] == 0)
            {
                points.Push(new Vector2(Xorigin, Yorigin - 1));
            }
            else
            {
                newDown = true;
            }

            while (right)
            {
                r++;
                if (!newUp)
                {
                    if (mapHeights[Xorigin + r, Yorigin + 1] < 0)
                    {
                        newUp = true;
                    }
                }
                else
                {
                    if (islands[Xorigin + r, Yorigin + 1] == 0 && mapHeights[Xorigin + r, Yorigin + 1] > 0)
                    {
                        points.Push(new Vector2(Xorigin + r, Yorigin + 1));
                        newUp = false;
                    }
                }


                if (!newDown)
                {
                    if (mapHeights[Xorigin + r, Yorigin - 1] < 0)
                    {
                        newDown = true;
                    }
                }
                else
                {
                    if (islands[Xorigin + r, Yorigin - 1] == 0 && mapHeights[Xorigin + r, Yorigin - 1] > 0)
                    {
                        points.Push(new Vector2(Xorigin + r, Yorigin - 1));
                        newDown = false;
                    }
                }





                if (islands[Xorigin + r, Yorigin] == 0 && mapHeights[Xorigin + r, Yorigin] > 0)
                {
                    islands[Xorigin + r, Yorigin] = island;
                }
                else
                {
                    right = false;
                }
            }
            newUp = false;
            newDown = false;
            if (mapHeights[Xorigin, Yorigin + 1] > 0 && islands[Xorigin, Yorigin + 1] == 0)
            {
            }
            else
            {
                newUp = true;
            }
            if (mapHeights[Xorigin, Yorigin - 1] > 0 && islands[Xorigin, Yorigin - 1] == 0)
            {
            }
            else
            {
                newDown = true;
            }









            while (left)
            {
                l++;
                if (!newUp)
                {
                    if (mapHeights[Xorigin - l, Yorigin + 1] < 0)
                    {
                        newUp = true;
                    }
                }
                else
                {
                    if (islands[Xorigin - l, Yorigin + 1] == 0 && mapHeights[Xorigin - l, Yorigin + 1] > 0)
                    {
                        points.Push(new Vector2(Xorigin - l, Yorigin + 1));
                        newUp = false;
                    }
                }

                if (!newDown)
                {
                    if (mapHeights[Xorigin - l, Yorigin - 1] < 0)
                    {
                        newDown = true;
                    }
                }
                else
                {
                    if (islands[Xorigin - l, Yorigin - 1] == 0 && mapHeights[Xorigin - l, Yorigin - 1] > 0)
                    {
                        points.Push(new Vector2(Xorigin - l, Yorigin - 1));
                        newDown = false;
                    }
                }















                if (islands[Xorigin - l, Yorigin] == 0 && mapHeights[Xorigin - l, Yorigin] > 0)
                {
                    islands[Xorigin - l, Yorigin] = island;
                }
                else
                {
                    left = false;
                }
            }
            islands[Xorigin, Yorigin] = island;

        }
    }


    Dictionary<int, float> islandHeights = new Dictionary<int, float>();
    void detectIslands()
    {
        Debug.Log("Starting Islands");

        for (int x = 0; x < chunkSize * mapSize; x++)
        {
            for (int y = 0; y < chunkSize * mapSize; y++)
            {
                if (islands[x, y] == 0 && mapHeights[x, y] > 0)
                {
                    island++;
                    FloodFillIsland(x, y);
                }
            }
        }

        for (int i = 0; i < island; i++)
        {
            islandHeights.Add(i + 1, 0f);
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                int check = islands[x, y];
                if (check > 0)
                {
                    float checkHeight = islandHeights[check];
                    if (mapHeights[x, y] > checkHeight)
                    {
                        islandHeights.Remove(check);
                        islandHeights.Add(check, mapHeights[x, y]);
                    }
                }
            }
        }
        
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (islands[x, y] > 0)
                {
                    if (mapHeights[x, y] > 0)
                    {
                        mapHeights[x, y] *= mapHeights[x, y] / islandHeights[islands[x, y]];
                    }
                }
            }
        }
    }


    /*


        


    */

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public void smooth(int iteration, float[,] array)
    {
  
        
            for (int i = 0; i < iteration; i++)
            {
                for (int x = 0; x <= chunkSize * mapSize; x++)
                {
                    for (int y = 0; y <= chunkSize * mapSize; y++)
                    {
                        if ((chunkSize * mapSize) - y > 0)
                        {
                            array[x, (chunkSize * mapSize) - y] = Mathf.Lerp(array[x, (chunkSize * mapSize) - y], array[x, ((chunkSize * mapSize) - y) - 1], Random.Range(0.4f, 0.6f));
                        }
                    }
                }

                for (int x = 0; x <= chunkSize * mapSize; x++)
                {
                    for (int y = 0; y <= chunkSize * mapSize; y++)
                    {
                        if ((chunkSize * mapSize) - y > 0)
                        {
                            array[(chunkSize * mapSize) - y, x] = Mathf.Lerp(array[(chunkSize * mapSize) - y, x], array[((chunkSize * mapSize) - y) - 1, x], Random.Range(0.4f, 0.6f));
                        }
                    }
                }
                for (int x = 0; x <= chunkSize * mapSize; x++)
                {
                    for (int y = 0; y <= chunkSize * mapSize; y++)
                    {
                        if (y + 1 < chunkSize * mapSize)
                        {
                            array[x, y] = Mathf.Lerp(array[x, y], array[x, y + 1], Random.Range(0.4f, 0.6f));
                        }
                    }
                }
                for (int x = 0; x <= chunkSize * mapSize; x++)
                {
                    for (int y = 0; y <= chunkSize * mapSize; y++)
                    {
                        if (y + 1 < chunkSize * mapSize)
                        {
                            array[y, x] = Mathf.Lerp(array[y, x], array[y + 1, x], Random.Range(0.4f, 0.6f));
                        }
                    }
                }
            }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    //might not need
    private void modifyMesh()
    {
        //vertZ[x,y] modified by each center polygon based off of distance.
        float max = 0f;
        float least = 1f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (PerlinNoise[x, y] == 0)
                {
                    VoronoiMapCusomNoise[x, y] = 0f;
                }
                if (VoronoiMapCusomNoise[x, y] > max) {
                    max = VoronoiMapCusomNoise[x, y];
                }
                if (VoronoiMapCusomNoise[x, y] < least && PerlinNoise[x,y] > 0)
                {
                    least = VoronoiMapCusomNoise[x, y];
                }
            }
        }
        
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                VoronoiMapCusomNoise[x, y] /= max;
            }
        }




        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if(PerlinNoise[x,y] > 0)
                {
                    mapHeights[x, y] = (maxHeight * (PerlinNoise[x, y] * VoronoiMapCusomNoise[x, y]));
                }
            }
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

        
    private float[,] moisture;
    private void genMoisture()
    {
        moisture = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        int fx, fy;
        fx = Random.Range(0, 2000000);
        fy = Random.Range(0, 2000000);



        for (int i = 0; i < 10; i++)
        {
            float temp = (1024 / Mathf.Pow(2, i - 1)) * 15;
            float s = Mathf.Pow(2, i - 1) * 2.0f;
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    moisture[x, y] += Mathf.PerlinNoise((getPos(x, y).x + fx) / temp, (getPos(x, y).z + fy) / temp) / s;
                }
            }
        }





        float m = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (moisture[x, y] > m)
                {
                    m = moisture[x, y];
                }
            }
        }
        
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                moisture[x, y] /= m;
            }
        }

        float l = 1f;
        m = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if(moisture[x,y] < l)
                {
                    l = moisture[x, y];
                }
                if(moisture[x,y] > m)
                {
                    m = moisture[x, y];
                }
            }
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                moisture[x, y] = Mathf.InverseLerp(l, m, moisture[x,y]);
            }
        }

    }

    float[,] VoronoiMapCusomNoise;
    private void genHeights(List<Polygon> map)
    {
        VoronoiMapCusomNoise = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                Vector3 temp = getPos(x, y);
                foreach (Polygon p in finalVoronoiMap)
                {
                    if (p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp, mapWH))
                        {
                            mapHeights[x, y] = 1;
                        }
                    }
                }
            }
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (mapHeights[x, y] == 0)
                {
                    mapHeights[x, y] = -15;
                    PerlinNoise[x, y] *= 0;
                }
            }
        }
        int iter = 1;

        bool modified = false;
        while (!modified)
        {
            modified = true;
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (mapHeights[x, y] != -15)
                    {
                        modified = false;
                    }
                }
            }
            float[,] temp = (float[,])mapHeights.Clone();
            bool fliy1 = false, fliy2 = false, flix1 = false, flix2 = false;



            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (!fliy1)
                    {
                        if (temp[x, y] > 0)
                        {
                            fliy1 = true;
                            VoronoiMapCusomNoise[x, y] = iter;
                            mapHeights[x, y] = -15;
                        }
                    }
                    else
                    {
                        if (temp[x, y] <= 0)
                        {
                            fliy1 = false;
                            VoronoiMapCusomNoise[x, y - 1] = iter;
                            mapHeights[x, y - 1] = -15;
                        }
                    }
                }
            }




            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (!flix1)
                    {
                        if (temp[y, x] > 0)
                        {
                            flix1 = true;
                            VoronoiMapCusomNoise[y, x] = iter;
                            mapHeights[y, x] = -15;
                        }
                    }
                    else
                    {
                        if (temp[y, x] <= 0)
                        {
                            flix1 = false;
                            VoronoiMapCusomNoise[y, x] = iter;
                            mapHeights[y, x] = -15;
                        }
                    }
                }
            }

            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (!fliy2)
                    {
                        if (temp[x, (chunkSize * mapSize) - y] > 0)
                        {
                            fliy2 = true;
                            VoronoiMapCusomNoise[x, (chunkSize * mapSize) - y] = iter;
                            mapHeights[x, (chunkSize * mapSize) - y] = -15;
                        }
                    }
                    else
                    {
                        if (temp[x, (chunkSize * mapSize) - y] <= 0)
                        {
                            fliy2 = false;
                            VoronoiMapCusomNoise[x, (chunkSize * mapSize) - (y + 1)] = iter;
                            mapHeights[x, (chunkSize * mapSize) - (y + 1)] = -15;
                        }
                    }
                }
            }
            
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (!flix2)
                    {
                        if (temp[(chunkSize * mapSize) - y, x] > 0)
                        {
                            flix2 = true;
                            VoronoiMapCusomNoise[(chunkSize * mapSize) - y, x] = iter;
                            mapHeights[(chunkSize * mapSize) - y, x] = -15;
                        }
                    }
                    else
                    {
                        if (temp[(chunkSize * mapSize) - y, x] <= 0 && ((chunkSize * mapSize) - (y + 1) > 0))
                        {
                            flix2 = false;
                            VoronoiMapCusomNoise[(chunkSize * mapSize) - (y + 1), x] = iter;
                            mapHeights[(chunkSize * mapSize) - (y + 1), x] = -15;
                        }
                    }
                }
            }
            iter++;
        }



        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                VoronoiMapCusomNoise[x, y] /= (float)iter;
            }
        }
        
    }


    /*













        





















        */

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>


    void genTexture()
    {
        float snowMod = 500f;
        Texture2D texture = new Texture2D(chunkSize * mapSize, chunkSize * mapSize);
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                Vector3 temp = getPos(x, y);
                float p = (Vector3.Distance(temp, new Vector3(proxyEquator.x, 0, proxyEquator.y)) - (biomeDistance / 2.5f)) / (biomeDistance / 2.5f);
                if (p > 1)
                {
                    p = 1f;
                }
                if (biome[x, y] >= biomes.Length)
                {
                    biome[x, y] = biomes.Length - 1;
                }
                if (biome[x, y] > 1)
                {
                    texture.SetPixel(x, y, Color.Lerp(biomes[biome[x, y]], Color.white, p));
                }
                else if (biome[x, y] == 1)
                {
                    texture.SetPixel(x, y, Color.Lerp(Color.yellow, Color.white, p));
                }
                else
                {
                    texture.SetPixel(x, y, biomes[biome[x, y]]);
                }
                if (mapHeights[x, y] >= snowMod)
                {
                    texture.SetPixel(x, y, Color.Lerp(texture.GetPixel(x, y), Color.white, (mapHeights[x, y] - snowMod) / (1100 - snowMod)));
                }
            }
        }
        texture.Apply();
        Materials[0].mainTexture = texture;
    }



    void genHeightTex()
    {
        Texture2D texture = new Texture2D(chunkSize * mapSize, chunkSize * mapSize);

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, PerlinNoise[x, y]));
            }
        }
        texture.Apply();
        Materials[0].mainTexture = texture;
    }

    void redTP()
    {
        Texture2D texture = new Texture2D(chunkSize * mapSize, chunkSize * mapSize);
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, PerlinNoise[x,y] * VoronoiMapCusomNoise[x, y]));
            }
        }

        texture.Apply();
        Materials[0].mainTexture = texture;
    }


    void genPTex()
    {
        Texture2D texture = new Texture2D(chunkSize * mapSize, chunkSize * mapSize);
        for (int i = 0; i < 10; i++)
        {
            float temp = (1024 / Mathf.Pow(2, i - 1)) * 15;
            float s = Mathf.Pow(2, i - 1) * 2.0f;
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    PerlinNoise[x, y] += Mathf.PerlinNoise((getPos(x, y).x + perlinNoiseXOffset) / temp, (getPos(x, y).z + perlinNoiseYOffset) / temp) / s;
                }
            }
        }






        float least = 110f;
        float most = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (PerlinNoise[x, y] > most)
                {
                    most = PerlinNoise[x, y];
                }
                if(PerlinNoise[x,y] < least)
                {
                    least = PerlinNoise[x, y];
                }
            }
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                PerlinNoise[x, y] = Mathf.InverseLerp(least, most, PerlinNoise[x, y]);
            }
        }



        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, PerlinNoise[x, y]));
            }
        }

        texture.Apply();
        Materials[0].mainTexture = texture;
    }


    private void moistureTex()
    {
        Texture2D texture = new Texture2D(chunkSize * mapSize, chunkSize * mapSize);
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                float m = moisture[x, y];
                if (m < 0.5f)
                {
                    texture.SetPixel(x, y, Color.Lerp(Color.yellow, new Color(0.627f, 0.321f, 0.176f), Mathf.InverseLerp(0, 0.5f, m)));
                }
                else if (m > 0.5f)
                {
                    texture.SetPixel(x, y, Color.Lerp(Color.green / 2, Color.green, Mathf.InverseLerp(0.5f, 1.0f, m)));
                }
            }
        }
        texture.Apply();
        Materials[0].mainTexture = texture;
    }


    

    private Vector3 getPos(int x, int y)
    {
        return new Vector3((x * chunkscale - (mapWH / 2)), 0, y * chunkscale - (mapWH / 2));
    }

    private Vector2 getXYFromPos(Vector3 Pos)
    {
        int x = (int)(Mathf.Ceil((Pos.x + (mapWH / 2)) / (chunkscale)));
        int y = (int)(Mathf.Ceil((Pos.z + (mapWH / 2)) / (chunkscale)));
        return new Vector2(x,y);
    }

    public float getHeight(int x, int y)
    {
        return mapHeights[x, y];
    }
}