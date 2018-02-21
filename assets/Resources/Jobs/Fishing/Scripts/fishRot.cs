using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishRot : MonoBehaviour {
	void Update () {
        transform.Rotate(new Vector3(0, Random.Range(0,.2f), 0));
	}
}
