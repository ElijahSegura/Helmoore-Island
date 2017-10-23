using UnityEngine;

public class Projector : MonoBehaviour {
    GameObject lookat;
    Vector3 pos;
    Vector3 prev;
	void Update () {
        if(lookat.transform.position != prev)
        {
            pos = lookat.transform.position;
            pos.y += .5f;
            transform.position = pos;
        }
	}

    public void setLookAt(GameObject l)
    {
        this.lookat = l;
    }
}
