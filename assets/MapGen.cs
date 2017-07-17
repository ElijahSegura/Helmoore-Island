using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using System.Linq;
using System.Runtime.InteropServices;



public class MapGen : MonoBehaviour
{
    public List<Chunk> sharableChunks;
    
    [SerializeField]
    private int
        m_pointCount = 380;
    public List<AnimationCurve> c = new List<AnimationCurve>();
    public AnimationCurve DesertCurve = new AnimationCurve();
    public List<Material> Materials = new List<Material>();
    private Map serverMap;//THIS IS WHAT WE WILL SEND OVER THE SERVER TO THE CLIENT, IT WILL HAVE MAPHEIGHT DATA, CURRENT CHUNK DATA, AND MAYBE SOME OTHER THINGS then we'll destroy the object that has this script to clean up some memory from the 2d arrays
    //blue is ocean, yellow is desert, green is plains, dark green is forest, purple is swamp
    private Color[] biomes = new Color[] { Color.blue, Color.yellow, Color.green, new Color(Color.green.r / 2, Color.green.g / 2, Color.green.b / 2), Color.red + Color.blue, Color.white };
    private Chunk[,] chunkMap;
    private int mapSize = 32;
    private int chunkSize = 32; //max size for chunksize is 250, 251 is over the mesh vertex limit
    private float it;
    public float chunkscale;
    private int rivers;
    private Vector2 proxyEquator;
    private int[,] biome;
    private List<Vector2> boundary = new List<Vector2>();
    private List<PerlinForms> riverForms = new List<PerlinForms>();
    private List<Vector2> tempRiver = new List<Vector2>();
    private List<Polygon> Map = new List<Polygon>();
    private List<Polygon> VoronoiMap = new List<Polygon>();
    private List<Vector2> m_points;
    public float mapWH = 50000;
    private float scale = 4.0F;
    private int xOffset, yOffset;
    private float biomeDistance = 0f;

    private List<Vector2> riverEnds = new List<Vector2>();
    private List<Vector2> secondaryPoints = new List<Vector2>();
    private List<Polygon> secondPolygons = new List<Polygon>();
    private List<Polygon> Definedvm = new List<Polygon>();

    private float maxHeight = 4096f;
    
    private int[,] islands;
    private float[,] vertZ;
    private Square[,] Squares;
    private float[,] xvertZ;
    private float[,] pNoise;
    public void run(int seedx, int seedy)
    {
        if (seedx >= 0 && seedy >= 0)
        {
            xOffset = seedx;
            yOffset = seedy;
        }
        else
        {
            xOffset = Random.Range(0, 2000000);
            yOffset = Random.Range(0, 2000000);
        }
        pNoise = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        mapV v = new mapV();
        
        proxyEquator = new Vector2(Random.Range(-(mapWH / 3), mapWH / 3), Random.Range(-(mapWH / 3), mapWH / 3));
        serverMap = new Map((chunkSize + 1) * mapSize);
        
        rivers = Random.Range(20, 40);
        it = 1;
        chunkscale = (mapWH / mapSize) / (chunkSize);
        genPTex();
        Demo();
        load();
    }

    public string getSeed()
    {
        return xOffset + "," + yOffset;
    }

    private void Demo()
    {
        List<uint> colors = new List<uint>();
        List<uint> colors2 = new List<uint>();
        List<uint> colors3 = new List<uint>();
        m_points = new List<Vector2>();

        for (int i = 0; i < m_pointCount; i++)
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


        



        Voronoi v = new Voronoi(m_points, colors, new Rect(-mapWH / 2, -mapWH / 2, mapWH, mapWH));
        Voronoi v2 = new Voronoi(secondaryPoints, colors2, new Rect(-mapWH / 2, -mapWH / 2, mapWH, mapWH));


        foreach (List<Vector2> l in v.Regions())
        {
            Polygon p = new Polygon(l);
            foreach (Vector2 vec in p.points())
            {
                Vector2 xy = getXYFromPos(new Vector3(vec.x, 0, vec.y));
                if(pNoise[(int)xy.x,(int)xy.y] > 0.2)
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
            foreach (List<Vector2> l in v2.Regions())
            {
                Polygon p2 = new Polygon(l);
                bool b = false;
                if(p.isInPolygon(p2.getCenter(), mapWH))
                {
                    Definedvm.Add(p2);
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
                                    Definedvm.Add(p2);
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
        vertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        Squares = new Square[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        xvertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        islands = new int[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        genHeights(Definedvm);
        smooth(5, vertZ);
        smooth(120, vnoise);
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

    public void loadChunks()
    {
        sharableChunks = new List<Chunk>();
        chunkMap = new Chunk[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, vertZ, Squares);
                if (!c.isflat())
                {
                    GameObject temp = new GameObject("Chunk: " + x + ":" + y);
                    temp.AddComponent<MeshRenderer>();
                    temp.GetComponent<MeshRenderer>().receiveShadows = true;
                    temp.AddComponent<MeshFilter>();
                    temp.GetComponent<MeshFilter>().mesh = c.getMesh();
                    temp.AddComponent<MeshCollider>();
                    temp.GetComponent<MeshCollider>().sharedMesh = c.getMesh();
                    temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                    temp.GetComponent<MeshRenderer>().material = Materials[0];
                    temp.transform.position = new Vector3(((x * (chunkSize * chunkscale)) - (mapWH / 2)), 0, ((y * (chunkSize * chunkscale)) - (mapWH / 2)));
                }
            }

        }
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
                if (vertZ[x, y] > 0 && biome[x, y] == 0)
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
                                if (vertZ[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] >= -4 && biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] != 8)
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
                if (vertZ[x, y] > 0)
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
                    if (vertZ[x, y] > 0)
                    {
                        biome[x, y - 1] = 1;
                        biome[x, y - 2] = 1;
                        biome[x, y] = 1;
                        start = true;
                    }
                }
                else
                {
                    if (vertZ[x, y] < 0)
                    {
                        biome[x, y - 1] = 1;
                        biome[x, y] = 1;
                        start = false;
                    }
                }



                if (!start2)
                {
                    if (vertZ[y, x] > 0)
                    {
                        biome[y, x - 1] = 1;
                        biome[y, x - 2] = 1;
                        biome[y, x] = 1;
                        start2 = true;
                    }
                }
                else
                {
                    if (vertZ[y, x] < 0)
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
                if (vertZ[x, y] < 1 && vertZ[x, y] > -5)
                {
                    boundary.Add(new Vector2((x * chunkscale - (mapWH / 2)), (y * chunkscale - (mapWH / 2))));
                }
                if (vertZ[x, y] > maxHeight)
                {
                    maxHeight = vertZ[x, y];
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
                if (vertZ[x, y] > 0)
                {
                    if (vertZ[x, y] > maxHeight)
                    {
                        maxHeight = vertZ[x, y];
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
            int X = (int)origin.x;
            int Y = (int)origin.y;
            int l = 0, r = 0;

            if (vertZ[X, Y + 1] > 0 && islands[X, Y + 1] == 0)
            {
                points.Push(new Vector2(X, Y + 1));
            }
            else
            {
                newUp = true;
            }
            if (vertZ[X, Y - 1] > 0 && islands[X, Y - 1] == 0)
            {
                points.Push(new Vector2(X, Y - 1));
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
                    if (vertZ[X + r, Y + 1] < 0)
                    {
                        newUp = true;
                    }
                }
                else
                {
                    if (islands[X + r, Y + 1] == 0 && vertZ[X + r, Y + 1] > 0)
                    {
                        points.Push(new Vector2(X + r, Y + 1));
                        newUp = false;
                    }
                }


                if (!newDown)
                {
                    if (vertZ[X + r, Y - 1] < 0)
                    {
                        newDown = true;
                    }
                }
                else
                {
                    if (islands[X + r, Y - 1] == 0 && vertZ[X + r, Y - 1] > 0)
                    {
                        points.Push(new Vector2(X + r, Y - 1));
                        newDown = false;
                    }
                }





                if (islands[X + r, Y] == 0 && vertZ[X + r, Y] > 0)
                {
                    islands[X + r, Y] = island;
                }
                else
                {
                    right = false;
                }
            }
            newUp = false;
            newDown = false;
            if (vertZ[X, Y + 1] > 0 && islands[X, Y + 1] == 0)
            {
            }
            else
            {
                newUp = true;
            }
            if (vertZ[X, Y - 1] > 0 && islands[X, Y - 1] == 0)
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
                    if (vertZ[X - l, Y + 1] < 0)
                    {
                        newUp = true;
                    }
                }
                else
                {
                    if (islands[X - l, Y + 1] == 0 && vertZ[X - l, Y + 1] > 0)
                    {
                        points.Push(new Vector2(X - l, Y + 1));
                        newUp = false;
                    }
                }

                if (!newDown)
                {
                    if (vertZ[X - l, Y - 1] < 0)
                    {
                        newDown = true;
                    }
                }
                else
                {
                    if (islands[X - l, Y - 1] == 0 && vertZ[X - l, Y - 1] > 0)
                    {
                        points.Push(new Vector2(X - l, Y - 1));
                        newDown = false;
                    }
                }















                if (islands[X - l, Y] == 0 && vertZ[X - l, Y] > 0)
                {
                    islands[X - l, Y] = island;
                }
                else
                {
                    left = false;
                }
            }
            islands[X, Y] = island;

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
                if (islands[x, y] == 0 && vertZ[x, y] > 0)
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
                    if (vertZ[x, y] > checkHeight)
                    {
                        islandHeights.Remove(check);
                        islandHeights.Add(check, vertZ[x, y]);
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
                    if (vertZ[x, y] > 0)
                    {
                        vertZ[x, y] *= c[0].Evaluate(vertZ[x, y] / islandHeights[islands[x, y]]);
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
                if (pNoise[x, y] == 0)
                {
                    vnoise[x, y] = 0f;
                }
                if (vnoise[x, y] > max) {
                    max = vnoise[x, y];
                }
                if (vnoise[x, y] < least && pNoise[x,y] > 0)
                {
                    least = vnoise[x, y];
                }
            }
        }
        
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                vnoise[x, y] /= max;
            }
        }




        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if(pNoise[x,y] > 0)
                {
                    vertZ[x, y] = (maxHeight * (pNoise[x, y] * vnoise[x, y]));
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

    float[,] vnoise;
    private void genHeights(List<Polygon> map)
    {
        vnoise = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                Vector3 temp = getPos(x, y);
                foreach (Polygon p in Definedvm)
                {
                    if (p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp, mapWH))
                        {
                            vertZ[x, y] = 1;
                        }
                    }
                }
            }
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (vertZ[x, y] == 0)
                {
                    vertZ[x, y] = -15;
                    pNoise[x, y] *= 0;
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
                    if (vertZ[x, y] != -15)
                    {
                        modified = false;
                    }
                }
            }
            float[,] temp = (float[,])vertZ.Clone();
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
                            vnoise[x, y] = iter;
                            vertZ[x, y] = -15;
                        }
                    }
                    else
                    {
                        if (temp[x, y] <= 0)
                        {
                            fliy1 = false;
                            vnoise[x, y - 1] = iter;
                            vertZ[x, y - 1] = -15;
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
                            vnoise[y, x] = iter;
                            vertZ[y, x] = -15;
                        }
                    }
                    else
                    {
                        if (temp[y, x] <= 0)
                        {
                            flix1 = false;
                            vnoise[y, x] = iter;
                            vertZ[y, x] = -15;
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
                            vnoise[x, (chunkSize * mapSize) - y] = iter;
                            vertZ[x, (chunkSize * mapSize) - y] = -15;
                        }
                    }
                    else
                    {
                        if (temp[x, (chunkSize * mapSize) - y] <= 0)
                        {
                            fliy2 = false;
                            vnoise[x, (chunkSize * mapSize) - (y + 1)] = iter;
                            vertZ[x, (chunkSize * mapSize) - (y + 1)] = -15;
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
                            vnoise[(chunkSize * mapSize) - y, x] = iter;
                            vertZ[(chunkSize * mapSize) - y, x] = -15;
                        }
                    }
                    else
                    {
                        if (temp[(chunkSize * mapSize) - y, x] <= 0 && ((chunkSize * mapSize) - (y + 1) > 0))
                        {
                            flix2 = false;
                            vnoise[(chunkSize * mapSize) - (y + 1), x] = iter;
                            vertZ[(chunkSize * mapSize) - (y + 1), x] = -15;
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
                vnoise[x, y] /= (float)iter;
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
                if (vertZ[x, y] >= snowMod)
                {
                    texture.SetPixel(x, y, Color.Lerp(texture.GetPixel(x, y), Color.white, (vertZ[x, y] - snowMod) / (1100 - snowMod)));
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
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, pNoise[x, y]));
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
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, pNoise[x,y] * vnoise[x, y]));
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
                    pNoise[x, y] += Mathf.PerlinNoise((getPos(x, y).x + xOffset) / temp, (getPos(x, y).z + yOffset) / temp) / s;
                }
            }
        }






        float l = 110f;
        float m = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (pNoise[x, y] > m)
                {
                    m = pNoise[x, y];
                }
                if(pNoise[x,y] < l)
                {
                    l = pNoise[x, y];
                }
            }
        }

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                pNoise[x, y] = Mathf.InverseLerp(l, m, pNoise[x, y]);
            }
        }



        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, pNoise[x, y]));
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
        return vertZ[x, y];
    }
}