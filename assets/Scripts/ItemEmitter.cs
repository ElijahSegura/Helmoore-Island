using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEmitter : MonoBehaviour {
    public GameObject give;
    public Vector3 force;
    private GameObject temp;
	// Use this for initialization
	void Start () {
        temp = GameObject.Instantiate(give);
        temp.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        temp.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180)));
        temp.transform.position = transform.position;
	}

    void Update()
    {
        if(temp == null)
        {
            temp = GameObject.Instantiate(give);
            temp.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            temp.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180)));
            temp.transform.position = transform.position;
        }
    }
}
