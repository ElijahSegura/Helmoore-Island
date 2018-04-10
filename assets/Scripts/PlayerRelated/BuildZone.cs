using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildZone : MonoBehaviour {
    private float buildArea = 20.0f;
    private float level = 1;
    [SerializeField]
    private GameObject constructionStorage;

    private Container buildingResources;
    // Use this for initialization
    void Start()
    {
        transform.GetChild(0).localScale = new Vector3((buildArea * 100f), (buildArea * 100f), 100f);
        constructionStorage = GameObject.Instantiate(constructionStorage);
        constructionStorage.transform.position = transform.position;
        buildingResources = constructionStorage.GetComponent<Container>();
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, Camera.main.GetComponent<PlayerCamera>().getPlayer().transform.position) < buildArea)
        {
            Camera.main.GetComponent<PlayerCamera>().setBuildable(true);
            Camera.main.GetComponent<PlayerCamera>().setBuildOrigin(transform.position, buildArea);
            Camera.main.GetComponent<PlayerCamera>().setBuildCrate(buildingResources);
        }
        else
        {
            Camera.main.GetComponent<PlayerCamera>().setBuildable(false);
            Camera.main.GetComponent<PlayerCamera>().buildingMenu.SetActive(false);
            Camera.main.GetComponent<PlayerCamera>().clearBuildCrate();
        }
    }
}
