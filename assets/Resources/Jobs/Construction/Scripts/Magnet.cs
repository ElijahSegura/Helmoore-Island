using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {

    public bool Base = false;

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag.Equals("Wall") && col.gameObject.transform.parent.gameObject != transform.parent.gameObject.transform.parent.gameObject)
        {
            Destroy(gameObject);
        }
        else if(col.gameObject.tag.Equals("Base") )
        {
            Destroy(gameObject);
        }
    }
}
