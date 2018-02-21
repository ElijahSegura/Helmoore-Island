using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloralWind : MonoBehaviour {
    private List<Vector3> verts;
    [SerializeField]
    private WindInfo info;
    private bool forward;
    [SerializeField]
    private AnimationCurve c;
    private float time = 0f;
    private List<Vector3> ogverts;
	// Use this for initialization
	void Start () {
        verts = new List<Vector3>(GetComponent<MeshFilter>().mesh.vertices);
        ogverts = verts;
        for (int i = 0; i < info.getVerts().Length; i++)
        {
            Temp = verts[info.getVerts()[i]];
            Temp.x = verts[info.getVerts()[i]].x + Random.Range(0.002f, -0.002f);
            ogverts[info.getVerts()[i]] = Temp;
        }
        time = Random.Range(0f, 1f);
        GetComponent<MeshFilter>().mesh.vertices = verts.ToArray();
        windMod = info.getWindLevel(transform.position);
    }

    // Update is called once per frame
    Vector3 Temp;
    private float windMod;
    private bool update = false;
	void Update () {
        if(GetComponent<Renderer>().isVisible)
        {
            if (update)
            {
                time += Time.deltaTime * windMod;
                foreach (int i in info.getVerts())
                {
                    Temp = verts[i];
                    Temp.x = verts[i].x + c.Evaluate(time);
                    verts[i] = Temp;
                }
                if (time >= 1)
                {
                    time = 0f;
                }
                GetComponent<MeshFilter>().mesh.vertices = verts.ToArray();
                update = false;
            }
            else
            {
                update = true;
            }
        }
    }
}
