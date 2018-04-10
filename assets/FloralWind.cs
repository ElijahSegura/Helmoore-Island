using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloralWind : MonoBehaviour {
    [SerializeField]
    private WindInfo info;
    [SerializeField]
    private AnimationCurve c;
    private float time = 0f;
	// Use this for initialization
	void Start () {
        time = Random.Range(0f, 1f);
        windMod = info.getWindLevel(transform.position);
    }

    // Update is called once per frame
    Vector3 Temp;
    private float windMod;
    public GameObject movePos;
	void Update () {
        time += (Time.deltaTime * windMod) / 2;
        movePos.transform.position = new Vector3(movePos.transform.position.x + c.Evaluate(time), movePos.transform.position.y, movePos.transform.position.z);
        if (time >= 1)
        {
            time = 0f;
        }
    }
}
