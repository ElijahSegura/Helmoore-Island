using UnityEngine;
using System.Collections.Generic;


public class Chunk {
    
    private List<Vector3> verts = new List<Vector3>();
    private List<Vector2> UVs = new List<Vector2>();
    private List<Square> squares = new List<Square>();
    private List<int> triangles = new List<int>();
    private Mesh mesh = new Mesh();
    private bool flat = true;
    
    public Chunk(int chunkSize, int mapsize, int posx, int posy, float chunkScale, float[,] vertZ)
    {
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
            int iterations = 0;
            for (int x = 0; x <= chunkSize; x++)
            {
                for (int y = 0; y <= chunkSize; y++)
                {
                    tX = x * chunkScale;
                    tY = y * chunkScale;
                    int px = (chunkSize * posx) + x;
                    int py = (chunkSize * posy) + y;
                    verts.Add(new Vector3(tX, vertZ[px, py], tY));
                    if(iterations % chunkSize == 0)
                    {
                        GameObject g = new GameObject(iterations + "");
                        g.transform.position = new Vector3(tX, 0, tY);
                    }
                    
                    UVs.Add(new Vector2((float)px / square, (float)py / square));

                    if(iterations + chunkSize + 2 < (chunkSize + 1) * (chunkSize + 1))
                    {
                        if(y != chunkSize)
                        {
                            triangles.Add(iterations);
                            triangles.Add(iterations + 1);
                            triangles.Add(iterations + chunkSize + 1);

                            triangles.Add(iterations + chunkSize + 1);
                            triangles.Add(iterations + 1);
                            triangles.Add(iterations + chunkSize + 2);
                        }
                    }
                    iterations++;
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
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();
        return mesh;
    }
}
