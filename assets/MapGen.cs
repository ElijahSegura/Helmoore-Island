using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using System.Linq;
using System.Runtime.InteropServices;



public class MapGen : MonoBehaviour
{
    [SerializeField]
    private int
        m_pointCount = 380;
    public List<AnimationCurve> c = new List<AnimationCurve>();

    public List<Material> Materials = new List<Material>();
    private Map serverMap;//THIS IS WHAT WE WILL SEND OVER THE SERVER TO THE CLIENT, IT WILL HAVE MAPHEIGHT DATA, CURRENT CHUNK DATA, AND MAYBE SOME OTHER THINGS then we'll destroy the object that has this script to clean up some memory from the 2d arrays
    //blue is ocean, yellow is desert, green is plains, dark green is forest, purple is swamp
    private Color[] biomes = new Color[]{ Color.blue, Color.yellow, Color.green, new Color(Color.green.r / 2, Color.green.g / 2, Color.green.b / 2), Color.red + Color.blue, Color.white };
    private Chunk[,] chunkMap;
    private int mapSize = 60;
    private int chunkSize = 20; //max size for chunksize is 250, 251 is over the mesh vertex limit
    private float it;
    private float chunkscale;
    private int rivers;
    private Vector2 proxyEquator;
    private int[,] biome;
    private List<Vector2> boundary = new List<Vector2>();
    private List<PerlinForms> riverForms = new List<PerlinForms>();

    private List<Polygon> Map = new List<Polygon>();
    private List<Polygon> VoronoiMap = new List<Polygon>();
    private List<Vector2> m_points;
        private float mapWH = 50000;
        private float scale = 180.0F;
    private int xOffset, yOffset;
    private float biomeDistance = 0f;

    private List<Vector2> riverEnds = new List<Vector2>();
    private List<Vector2> secondaryPoints = new List<Vector2>();
    private List<Polygon> secondPolygons = new List<Polygon>();
    private List<Polygon> Definedvm = new List<Polygon>();

    private int[,] islands;
    private float[,] vertZ;
    private Square[,] Squares;
    private float[,] xvertZ;

    void Start()
    {
        proxyEquator = new Vector2(Random.Range(-(mapWH / 3), mapWH / 3), Random.Range(-(mapWH / 3), mapWH / 3));
        Debug.Log(biomes.Length);
        serverMap = new Map((chunkSize + 1) * mapSize);
        rivers = Random.Range(40, 60);
        it = 1;
        chunkscale = (mapWH / mapSize) / (chunkSize);
        xOffset = Random.Range(-1000000, 1000000);
        yOffset = Random.Range(-1000000, 1000000);
        Demo();
        load();
    }

    private void Demo()
    {
        List<uint> colors = new List<uint>();
        List<uint> colors2 = new List<uint>();
        m_points = new List<Vector2>();

        for (int i = 0; i < m_pointCount; i++)
        {
            colors.Add(0);
            m_points.Add(new Vector2(
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2),
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2))
            );
        }

        for (int i = 0; i < 2000; i++)
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
            if (Mathf.PerlinNoise((p.getCenter().x + xOffset) / scale, (p.getCenter().z + yOffset) / scale) > 0.1F)
            {
                if ((Vector3.Distance(p.getCenter(), Vector3.zero) < (mapWH / 3.5) || Vector3.Distance(p.getCenter() + new Vector3(Random.Range(-(mapWH / 8), (mapWH / 8)), 0, Random.Range(-(mapWH / 8), (mapWH / 8))), Vector3.zero) < (mapWH / 2.666F)) && Vector3.Distance(p.getCenter(), Vector3.zero) < mapWH * .9f)
                {
                    Map.Add(p);
                }
            }
        }

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
                foreach (Vector2 VP in p2.points())
                {
                    Vector3 tempV = new Vector3(VP.x, 0, VP.y);
                    if (!b)
                    {
                        if (p.isAroundPolygon(tempV))
                        {
                            if (p.isInPolygon(tempV))
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
        genHeights();
        modifyMesh();
        smooth();
        detectIslands();
        genRivers();
        genBiomes();
        genSquares();
        genTexture();
        loadChunks();
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    public void loadChunks()
    {
        chunkMap = new Chunk[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, vertZ, riverForms, Squares);
                if(!c.isflat())
                {
                    GameObject temp = new GameObject("Chunk: " + x + ":" + y);
                    temp.AddComponent<MeshRenderer>();
                    temp.GetComponent<MeshRenderer>().receiveShadows = true;
                    temp.AddComponent<MeshFilter>();
                    temp.GetComponent<MeshFilter>().mesh = c.getMesh();
                    temp.AddComponent<MeshCollider>();
                    temp.GetComponent<MeshCollider>().sharedMesh = c.getMesh();
                    temp.GetComponent<MeshFilter>().mesh.Optimize();
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
                            if(vertZ[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] >= -4 && biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] != 8)
                            {
                                Vector3 temp = new Vector3((((x + i) - (distanceModifier / 2)) * chunkscale - (mapWH / 2)), 0, (((y + j) - (distanceModifier / 2)) * chunkscale - (mapWH / 2)));
                                if (Vector3.Distance(v, temp) < Random.Range(750f, 1000f))
                                {
                                    if(islands[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] == isl)
                                    {
                                        biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] = 3;
                                    }
                                }
                                else if (Vector3.Distance(v, temp) < Random.Range(2500f, 3500f))
                                {
                                    if (biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] != 3)
                                    {
                                        if(islands[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] == isl)
                                        {
                                            biome[(x + i) - (distanceModifier / 2), (y + j) - (distanceModifier / 2)] = 2;
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
                if(biome[x,y] == 8)
                {
                    biome[x, y] = 0;
                }
                if (vertZ[x, y] > 0)
                {
                    Vector3 temp = new Vector3(((x) * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
                    if(Vector3.Distance(temp, new Vector3(proxyEquator.x, 0, proxyEquator.y)) > biomeDistance) 
                    {
                        biomeDistance = Vector3.Distance(temp, new Vector3(proxyEquator.x, 0, proxyEquator.y));
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
                    if(closest10.Count >= 10)
                    {
                        closest10.RemoveAt(0);
                    }
                }
            }
            int ra = Random.Range(0, closest10.Count - 1);
            Vector3 temp = new Vector3(closest10[ra].x, 0 , closest10[ra].y);
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
                if(!newUp)
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


                if(!newDown)
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
                if(!newUp)
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

                if(!newDown)
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
    List<LineSegment> islandG = new List<LineSegment>();
    void detectIslands()
    {
        Debug.Log("Starting Islands");
        
        for (int x = 0; x < chunkSize * mapSize; x++)
        {
            for (int y = 0; y < chunkSize * mapSize; y++)
            {
                if(islands[x,y] == 0 && vertZ[x,y] > 0)
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
                if(check > 0)
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

        for (int i = 0; i < island; i++)
        {
            Debug.Log(islandHeights[i + 1]);
        }


        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if(islands[x,y] > 0)
                {
                    if (vertZ[x, y] > 0)
                    {
                        if(islandHeights[islands[x,y]] > 600)
                        {
                            vertZ[x, y] *= c[0].Evaluate(vertZ[x, y] / islandHeights[islands[x, y]]);
                        }
                        else
                        {
                            vertZ[x, y] *= c[1].Evaluate(vertZ[x, y] / islandHeights[islands[x, y]]);
                        }
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

    public void smooth()
    {
        for (int i = 0; i < 60; i++)
        {
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if ((chunkSize * mapSize) - y > 0)
                    {
                        vertZ[x, (chunkSize * mapSize) - y] = Mathf.Lerp(vertZ[x, (chunkSize * mapSize) - y], vertZ[x, ((chunkSize * mapSize) - y) - 1], Random.Range(0.4f, 0.6f));
                    }
                }
            }

            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if ((chunkSize * mapSize) - y > 0)
                    {
                        vertZ[(chunkSize * mapSize) - y, x] = Mathf.Lerp(vertZ[(chunkSize * mapSize) - y, x], vertZ[((chunkSize * mapSize) - y) - 1, x], 0.5f);
                    }
                }
            }
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (y + 1 < chunkSize * mapSize)
                    {
                        vertZ[x, y] = Mathf.Lerp(vertZ[x, y], vertZ[x, y + 1], 0.5f);
                    }
                }
            }
            for (int x = 0; x <= chunkSize * mapSize; x++)
            {
                for (int y = 0; y <= chunkSize * mapSize; y++)
                {
                    if (y + 1 < chunkSize * mapSize)
                    {
                        vertZ[y, x] = Mathf.Lerp(vertZ[y, x], vertZ[y + 1, x], 0.5f);
                    }
                }
            }
        }

        float max = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (vertZ[x, y] > max)
                {
                    max = vertZ[x, y];
                }
            }
        }
        max += 15f;
        
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if (vertZ[x, y] > 0)
                {
                    vertZ[x, y] = Mathf.Lerp(-15, 1100 + (200 * Mathf.PerlinNoise((x * chunkscale - (mapWH / 2)) / (scale * 2), (y * chunkscale - (mapWH / 2)) )) / (scale * 2), (vertZ[x, y] + 15f) / max);
                }
                
            }
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private void modifyMesh()
    {
        float per = 0;
        float max = 0f;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if ((vertZ[x, y]) < (xvertZ[x, y]))
                {
                    per = (vertZ[x, y]) / (xvertZ[x, y]);
                    vertZ[x, y] = vertZ[x, y];
                }
                else if ((vertZ[x, y]) > (xvertZ[x, y]))
                {
                    per = (xvertZ[x, y]) / (vertZ[x, y]);
                    vertZ[x, y] = (xvertZ[x, y]);
                }
                if (vertZ[x, y] > max)
                {
                    max = vertZ[x, y];
                }
            }
        }


        
        max += 15f;
        

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                if(vertZ[x,y] > 0)
                {
                    vertZ[x, y] = Mathf.Lerp(-15, 1100, (vertZ[x, y] + 15f) / max);
                    Vector3 temp = new Vector3((x * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
                    //vertZ[x, y] += 240 * Mathf.PerlinNoise((temp.x + xOffset) / 400.1f, (temp.z + yOffset) / 400.1f);
                }
                if(vertZ[x,y] == 0)
                {
                    vertZ[x, y] = -15;
                }
            }
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private void genHeights()
    {
        float inPoly = 0;
        float inPoly2 = 0;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                bool added = false;
                bool added2 = false;
                Vector3 temp = new Vector3((x * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
                Vector3 temp2 = new Vector3((y * chunkscale - (mapWH / 2)), 0, (x * chunkscale - (mapWH / 2)));
                foreach (Polygon p in Definedvm)
                {
                    if (p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp))
                        {
                            if (!added)
                            {
                                inPoly += it;
                                vertZ[x, y] = inPoly;
                                added = true;
                            }
                        }
                    }
                    if (p.isAroundPolygon(temp2))
                    {
                        if (p.isInPolygon(temp2))
                        {
                            if (!added2)
                            {
                                inPoly2 += it;
                                xvertZ[y, x] = inPoly2;
                                added2 = true;
                            }
                        }
                    }
                }
                if (!added)
                {
                    for (int i = 0; i < inPoly; i++)
                    {

                        if (i > inPoly / 2)
                        {
                            vertZ[x, y - ((int)inPoly - i)] = inPoly - (i * it);
                        }
                    }
                    inPoly = 0;
                    vertZ[x, y] = 0;
                }
                if (!added2)
                {
                    for (int i = 0; i < inPoly2; i++)
                    {

                        if (i > inPoly2 / 2)
                        {
                            xvertZ[y - ((int)inPoly2 - i), x] = inPoly2 - (i * it);
                        }
                    }
                    inPoly2 = 0;
                    xvertZ[y, x] = 0;
                }
            }
        }
    }

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
                Vector3 temp = new Vector3((x * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
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
}
