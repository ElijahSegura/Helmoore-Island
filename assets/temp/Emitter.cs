using UnityEngine;
using System.Collections;

public class Emitter : MonoBehaviour {

    public GameObject item;
    int i = 0;
    int max;
    int items = 0;
    void Start()
    {
        max = Random.Range(3, 10);
    }

	// Update is called once per frame
	void Update()
    {
        if (items < max)
        {
            GameObject t = GameObject.Instantiate(item);
            t.transform.position = transform.position;
            items++;
        }
        i++;
        if(items >= max)
        {
            Destroy(gameObject);
        }
    }
}
