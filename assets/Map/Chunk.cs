using UnityEngine;
using System.Collections.Generic;

public class Chunk {
    
    private Vector3[] verts;
    private Vector2[] UVs;
    private List<int> triangles = new List<int>();
    private Mesh mesh = new Mesh();
    private bool flat = true;
    
    public Chunk(int chunkSize, int mapsize, int posx, int posy, float chunkScale, float[,] vertZ)
    {
        verts = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        UVs = new Vector2[(chunkSize + 1) * (chunkSize + 1)];
        float tX;
        float tY;
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

        








        int vIndex = 0;
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
                    verts[vIndex] = new Vector3(tX, vertZ[px, py], tY);
                    UVs[vIndex] = new Vector2((float)px / square, (float)py / square);
                    vIndex++;

                    if (iterations + chunkSize + 2 < (chunkSize + 1) * (chunkSize + 1))
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
        mesh.vertices = verts;
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs;
        return mesh;
    }
}
