using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishUpDown : MonoBehaviour {
    public AnimationCurve c;
    private float time = 0f;
    Vector3 t;
    void Start()
    {
        time = Random.Range(0f, 1f);
        t = transform.position;
    }
    Vector3 n;
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime / 20f;
        n = t;
        n.y = t.y + c.Evaluate(time);
        transform.position = n;
        if(time >= 1f)
        {
            time = 0f;
        }
	}
}
