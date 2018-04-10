using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grassgen : MonoBehaviour {
    private int width = 30, height = 30;
    [SerializeField]
    private GameObject grassPrefab;
    private float range = 2f;//in both directions

    RaycastHit info;
    Vector3 p;
    GameObject temp;
    private float min = 1f;
	void Start () {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Physics.Raycast(transform.position + new Vector3((((x * 2f) - (width)) + Random.Range(-1f, 1f)), 0, (((y * 2f) - (height)) + Random.Range(-1f, 1f))), -transform.up, out info, 1f))
                {
                    if(info.collider.tag.Equals("Ground"))
                    {
                        p = info.point;
                        p.y += 0.38f + Random.Range(-0.2f, 0.1f);
                        temp = GameObject.Instantiate(grassPrefab);
                        temp.transform.position = p;
                        temp.transform.Rotate(new Vector3(0, Random.Range(360f, -360f), 0));
                    }
                }
            }
        }
	}
}
