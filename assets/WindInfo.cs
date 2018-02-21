using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInfo : MonoBehaviour {
    //6,7,8 are top
    private int[] edit = { 0, 3, 4, 7, 8, 11, 12, 15, 16, 19, 22, 23, 24, 27, 28, 31 };
    public int[] getVerts()
    {
        return edit;
    }

    public float getWindLevel(Vector3 pos)
    {
        return Mathf.PerlinNoise(pos.x / scale, pos.z / scale);
    }

    private float offX = 0f, offY = 0f;
    private float time = 0f;
    private float scale = 200f;
}
