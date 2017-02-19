using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using System.Linq;

public class VoronoiDemo : MonoBehaviour
{
	[SerializeField]
	private int
		m_pointCount = 380;// tested and 380 and 4000 seem like a good match
    public GameObject mapPiece;
    private Chunk[,] chunkMap;
    private int mapSize = 3;
    private int chunkSize = 100; //max size for chunksize is 250, 251 is over the mesh vertex limit
    private float chunkscale;

    public GameObject Temp;

    private List<Vector2> boundary = new List<Vector2>();

    private List<Vector2> onEdge = new List<Vector2>();
    private List<Polygon> Map = new List<Polygon>();
    private List<Polygon> VoronoiMap = new List<Polygon>();
    private int maxHeight = 10;
    private List<Vector2> PerlinNoisePoints = new List<Vector2>();
	private List<Vector2> m_points;
	private float mapWH = 1000;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
    private List<Polygon> polygons = new List<Polygon>();
    private float scale = 180.0F;
    private Dictionary<Vector2, int> vmp = new Dictionary<Vector2, int>();
    private int xOffset, yOffset;



    private List<Vector2> secondaryPoints = new List<Vector2>();
    private List<Polygon> secondPolygons = new List<Polygon>();
    private List<Polygon> Definedvm = new List<Polygon>();

    float greatedLeast = 0;

   
	void Start ()
	{
        chunkscale = (mapWH / mapSize) / (chunkSize);
        xOffset = Random.Range(-200000, 200000);
        yOffset = Random.Range(-200000, 200000);
		Demo();
        load();
	}

	private void Demo ()
	{
		List<uint> colors = new List<uint> ();
        List<uint> colors2 = new List<uint>();
		m_points = new List<Vector2> ();
			
		for (int i = 0; i < m_pointCount; i++) {
			colors.Add (0);
			m_points.Add (new Vector2 (
					UnityEngine.Random.Range (-mapWH / 2, mapWH / 2),
					UnityEngine.Random.Range (-mapWH / 2, mapWH / 2))
			);
		}

        for (int i = 0; i < 4000; i++)
        {
            colors2.Add(0);
            secondaryPoints.Add(new Vector2(
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2),
                    UnityEngine.Random.Range(-mapWH / 2, mapWH / 2))
            );
        }

        PerlinNoisePoints = m_points;
		Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, colors, new Rect (-mapWH / 2, -mapWH / 2, mapWH, mapWH));
        Voronoi v2 = new Voronoi(secondaryPoints, colors2, new Rect(-mapWH / 2, -mapWH / 2, mapWH, mapWH));
        m_edges = v.VoronoiDiagram();



        foreach(List<Vector2> l in v.Regions())
        {
            Polygon p = new Polygon(l);
            polygons.Add(p);
        }
        foreach (Polygon p in polygons)
        {
            if (Mathf.PerlinNoise((p.getCenter().x + xOffset) / scale, (p.getCenter().z + yOffset) / scale) > 0.1F)
            {
                //
                if (Vector3.Distance(p.getCenter(), Vector3.zero) < (mapWH / 3.5) || Vector3.Distance(p.getCenter() + new Vector3(Random.Range(-(mapWH / 8), (mapWH / 8)), 0, Random.Range(-(mapWH / 8), (mapWH / 8))), Vector3.zero) < (mapWH / 2.666F))
                {
                    Map.Add(p);
                }
            }
        }

        foreach(Polygon p in Map)
        {
            foreach(Polygon p2 in Map)
            {
                if(!p.Equals(p2))
                {
                    p.testConnected(p2);
                }
            }
        }

        foreach(Polygon p in Map) {
            if(p.getConnections() > 0 && p.getIsland().ToArray()[0].getConnections() > 1)
            {
                VoronoiMap.Add(p);
            }
        }


        foreach (List<Vector2> l in v2.Regions())
        {
            Polygon p = new Polygon(l);
            secondPolygons.Add(p);
        }

        foreach (Polygon p in VoronoiMap)
        {
            foreach (Polygon p2 in secondPolygons)
            {
                if (p.isAroundPolygon(p2.getCenter()))
                {
                    Definedvm.Add(p2);
                }
            }
        }


        /*
        Alright, so what's going on is i'm adding all of the vertex's into one array, then im trying to only get the ones that
        have 1 of that vertex, that would mean that it is part of the boundry. however, for some reason it still displays the points that have 2 or
        3 of the same vertex, even though it lists them BOTH with the Debug.log and there will be two Capsules, that i use as my place marker to 
        see where the outline will be at that point. sometimes there are more than two, sometimes there's only one, 
        i tried it with a dictionary<Vector2, int> and it reproduces the same thing.

        */
        foreach (Polygon p in Definedvm)
        {
            foreach (Vector2 v3 in p.points())
            {
                try
                {
                    string tempx = v3.x.ToString().Substring(0, v3.x.ToString().IndexOf(".") + 2);
                    string tempy = v3.y.ToString().Substring(0, v3.y.ToString().IndexOf(".") + 2);
                    Vector2 t = new Vector2(float.Parse(tempx), float.Parse(tempy));
                    boundary.Add(t);
                }
                catch (System.Exception)
                {
                    string tempx = v3.x.ToString();
                    string tempy = v3.y.ToString();
                    Debug.Log(tempx);
                    Debug.Log(tempy);
                    float x = v3.x;
                    float y = v3.y;
                    if (!tempx.Contains(".0"))
                    {
                        x = (int)v3.x;
                    }
                    else
                    {
                        x = float.Parse(v3.x.ToString().Substring(0, v3.x.ToString().IndexOf(".") + 2));
                    }

                    if (!tempy.Contains(".0"))
                    {
                        x = (int)v3.y;
                    }
                    else
                    {
                        y = float.Parse(v3.y.ToString().Substring(0, v3.y.ToString().IndexOf(".") + 2));
                    }

                    Vector2 t = new Vector2(x, y);
                    boundary.Add(t);
                }
                
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Polygon p in Definedvm)
        {
            Vector2[] temparray = p.points().ToArray();
            for (int i = 0; i < temparray.Length; i++)
            {
                if (i + 1 < temparray.Length)
                {
                    Vector3 left = new Vector3(temparray[i].x, 0, temparray[i].y);
                    Vector3 right = new Vector3(temparray[i + 1].x, 0, temparray[i + 1].y);
                    left.Set(left.x, maxHeight * Mathf.PerlinNoise(left.x / scale, left.z / scale), left.z);
                    right.Set(right.x, maxHeight * Mathf.PerlinNoise(right.x / scale, right.z / scale), right.z);
                    Gizmos.DrawLine(left, right);
                }
            }
            Vector3 left2 = new Vector3(temparray[0].x, 0, temparray[0].y);
            Vector3 right2 = new Vector3(temparray[temparray.Length - 1].x, 0, temparray[temparray.Length - 1].y);
            left2.Set(left2.x, maxHeight * Mathf.PerlinNoise(left2.x / scale, left2.z / scale), left2.z);
            right2.Set(right2.x, maxHeight * Mathf.PerlinNoise(right2.x / scale, right2.z / scale), right2.z);
            Gizmos.DrawLine(left2, right2);
        }

    }
    /*
    
    */
    private void drawAllLines()
    {

        



        if (m_edges != null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector3 left = new Vector3(((Vector2)m_edges[i].p0).x, 0, ((Vector2)m_edges[i].p0).y);
                Vector3 right = new Vector3(((Vector2)m_edges[i].p1).x, 0, ((Vector2)m_edges[i].p1).y);
                left.Set(left.x, maxHeight * Mathf.PerlinNoise(left.x / scale, left.z / scale), left.z);
                right.Set(right.x, maxHeight * Mathf.PerlinNoise(right.x / scale, right.z / scale), right.z);
                Gizmos.DrawLine(left, right);
            }
        }
    }

    Dictionary<Vector2, Vector3> test = new Dictionary<Vector2, Vector3>();
    float[,] vertZ;
    private void load()
    {
        vertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        float inPoly = 0;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                bool added = false;
                Vector3 temp = new Vector3((x * chunkscale - (mapWH / 2)), 0, (y * chunkscale - (mapWH / 2)));
                foreach (Polygon p in Definedvm)
                {
                    if (p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp))
                        {
                            if (!added)
                            {
                                inPoly++;
                                vertZ[x, y] = inPoly;
                                added = true;
                            }
                        }
                    }
                }
                if (!added)
                {
                    for (int i = 0; i < inPoly; i++)
                    {
                        if(i > inPoly / 2)
                        {
                            vertZ[x, y - ((int)inPoly - i)] = inPoly - i;
                        }
                    }
                    inPoly = 0;
                }
            }
        }
        float[,] xvertZ = new float[(chunkSize + 1) * mapSize, (chunkSize + 1) * mapSize];
        inPoly = 0;
        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                bool added = false;
                Vector3 temp = new Vector3((y * chunkscale - (mapWH / 2)), 0, (x * chunkscale - (mapWH / 2)));
                foreach (Polygon p in Definedvm)
                {
                    if (p.isAroundPolygon(temp))
                    {
                        if (p.isInPolygon(temp))
                        {
                            if (!added)
                            {
                                inPoly++;
                                xvertZ[y, x] = inPoly;
                                added = true;
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
                            xvertZ[y - ((int)inPoly - i), x] = inPoly - i;
                        }
                    }
                    inPoly = 0;
                    xvertZ[y, x] = 0;
                }
            }
        }
        float maxHeight = 0;

        for (int x = 0; x <= chunkSize * mapSize; x++)
        {
            for (int y = 0; y <= chunkSize * mapSize; y++)
            {
                vertZ[x, y] = Mathf.Abs(vertZ[x, y] - (xvertZ[x, y] - 1));
                if (vertZ[x, y] > maxHeight)
                {
                    maxHeight = vertZ[x, y];
                }
            }
        }


        chunkMap = new Chunk[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                chunkMap[x, y] = new Chunk(chunkSize, x, y, chunkscale, vertZ, maxHeight);
                GameObject temp = GameObject.Instantiate(mapPiece);
                temp.transform.position = new Vector3((x * (chunkSize * chunkscale)) - (mapWH / 2), 0, (y * (chunkSize * chunkscale)) - (mapWH / 2));
                temp.GetComponent<MeshGen>().loadChunk(chunkMap[x, y], x, y);
                temp.name = x + y + "";
            }
        }
    }

}


/*
inPoly++;
                                int a = mapSize - 1;
                                int b = mapSize - 1;
                                
                                chunkMap[a, b].setVert(x - (chunkSize * a), y - (chunkSize * b), inPoly);
*/
