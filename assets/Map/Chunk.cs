using UnityEngine;
using System.Collections.Generic;


public class Chunk {
    
    private List<Vector3> verts = new List<Vector3>();
    private List<Vector2> UVs = new List<Vector2>();
    private List<Square> tris = new List<Square>();
    private List<int> temp = new List<int>();
    private Mesh mesh = new Mesh();
    private bool flat = true;
    public int posx;
    public int posy;

    public Chunk(int chunkSize, int mapsize, int posx, int posy, float chunkScale, float[,] vertZ, List<PerlinForms> rivers, Square[,] Squares)
    {
        this.posx = posx;
        this.posy = posy;
        float tX;
        float tY;
        float value;
        value = vertZ[(chunkSize * posx) + 0, (chunkSize * posy) + 0];
        for (int x = 0; x <= chunkSize; x++)
        {
            for (int y = 0; y <= chunkSize; y++)
            {
                int px = (chunkSize * posx) + x;
                int py = (chunkSize * posy) + y;
                if (vertZ[px, py] > -14)
                {
                    flat = false;
                    break;
                }
            }
        }
        float square = (chunkSize * mapsize);
        if (!flat)
        {
            for (int x = 0; x <= chunkSize; x++)
            {
                for (int y = 0; y <= chunkSize; y++)
                {
                    tX = x * chunkScale;
                    tY = y * chunkScale;
                    int px = (chunkSize * posx) + x;
                    int py = (chunkSize * posy) + y;
                    verts.Add(new Vector3(tX, vertZ[px, py], tY));
                    UVs.Add(new Vector2((float)px / square, (float)py / square));
                    if (x < chunkSize && y < chunkSize)
                    {
                        if (Squares[px, py].getEnabled())
                        {
                            foreach (int i in Squares[px, py].getTriangles())
                            {
                                temp.Add(i);
                            }
                        }
                    }
                }
            }
        }
        
    }

    public bool isflat()
    {
        return flat;
    }
    
    public Mesh getMesh()
    {
        mesh.vertices = verts.ToArray();
        mesh.triangles = temp.ToArray();
        mesh.uv = UVs.ToArray();
        return mesh;
    }
}
