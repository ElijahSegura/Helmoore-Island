using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldButton : MonoBehaviour {
    public bool destroy = false;
    public bool interact = false;
    public GameObject eventObject;

    void OnTriggerEnter(Collider col)
    {
        if (destroy)
        {
            eventObject.GetComponent<EnvironmentTrigger>().trigger();
            Destroy(eventObject);
        }
    }
}
