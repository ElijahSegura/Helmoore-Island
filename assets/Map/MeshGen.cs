using UnityEngine;
using System.Collections.Generic;

public class MeshGen : MonoBehaviour {


    public void loadChunk(Chunk c, int x, int y)
    {
        MeshFilter mesh = GetComponent<MeshFilter>();
        mesh.mesh = c.getMesh();
        mesh.mesh.name = "Chunk" + x + y;
        GetComponent<MeshCollider>().sharedMesh = mesh.mesh;
        mesh.mesh.Optimize();
        mesh.mesh.RecalculateNormals();
    }

    //63, 126, 252, 504

    
}
