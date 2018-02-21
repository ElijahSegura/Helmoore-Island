using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour {
    public int portions;
    public int updateframes;
    public GameObject top, bottom;

    private List<Vector3> verts = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private GameObject trail;
    public Material m;
    
    void Start()
    {
        trail = new GameObject("Trial");
        trail.AddComponent<MeshFilter>();
        trail.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        trail.GetComponent<MeshFilter>().mesh = mesh;
        trail.GetComponent<MeshRenderer>().material = m;

    }
    // Update is called once per frame
    private int frame = 0;

    private Vector3 prevT;
    private int madePortions = 0;
    private int count = 0;
    int vs = 0;
	void Update () {
        
        frame++;
        foreach (Vector3 v in verts)
        {
            if(vs > 1)
            {
                verts[vs] = v - (transform.position - prevT);
                vs++;
            }
            else
            {
                vs++;
            }
        }
        
        //odd = bottom
        //even = top
        

        vs = 0;
        if (frame == updateframes)
        {
            count = verts.Count;
            verts.Add(top.transform.position - transform.position);
            verts.Add(bottom.transform.position - transform.position);

            frame = 0;
            madePortions++;
            if(madePortions >= portions)
            {
                verts.RemoveAt(0);
                verts.RemoveAt(0);
            }
            else
            {
                if(count > 2)
                {
                    triangles.Add(count - 4);
                    triangles.Add(count - 3);
                    triangles.Add(count - 2);
                    triangles.Add(count - 2);
                    triangles.Add(count - 3);
                    triangles.Add(count - 1);
                }
            }
            for (int i = 0; i < verts.Count; i += 2)
            {
                verts[i + 1] = Vector3.Lerp(verts[i], verts[i + 1], ((float)(i) / verts.Count));
            }
        }
        updateMesh();
        prevT = transform.position;
        trail.transform.position = bottom.transform.position;
    }
    
    public void setCombat(bool combat)
    {
        this.combatSwing = combat;
    }

    private bool combatSwing = false;
    private Mesh mesh;
    public void updateMesh()
    {
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
