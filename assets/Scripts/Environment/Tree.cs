using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : EnvironmentTrigger {

	// Use this for initialization
	void Start () {
		
	}
    public GameObject log;
    

    public override void trigger()
    {
        log = GameObject.Instantiate(log);
        Vector3 pos = transform.position;
        pos.y += 1;
        log.transform.position = pos;
        for(int i = 0; i < Random.Range(20,30); i++)
        {
            GameObject temp = GameObject.Instantiate(log);
            log.GetComponent<Item>().addToStack(temp.GetComponent<Item>());
            Destroy(temp);
        }
    }
}
