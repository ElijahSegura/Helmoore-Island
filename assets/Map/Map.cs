using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour {
    public GameObject mapPiece;
    private GameObject MyCharacter;
    public GameObject Player;
    public int chunkSize = 250;
    public static int mapSize = 3;
    private Chunk[,] map = new Chunk[mapSize, mapSize];
    private int numofpoi = 1;
    public GameObject Grass;
    private Vector2 poi;
    public List<GameObject> lakes = new List<GameObject>();
    int center = mapSize / 2;

    private int po, so, to;

    void Start()
    {
        po = Random.Range(-50000, 50000);
        so = Random.Range(-50000, 50000);
        to = Random.Range(-50000, 50000);

        poi = new Vector2(0, 0);
        Debug.Log(poi);
        GameObject player = Player;
        player.transform.position = new Vector3(0, 260, 0);
        player = GameObject.Instantiate(player);
        MyCharacter = player;
        //63, 126, 252, 504
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                //map[x, y] = new Chunk(chunkSize, Grass, lakes.ToArray()[0], center - x, center - y, poi, (chunkSize * mapSize) / (1.8F * numofpoi), po, so, to);
                //map[x, y] = null;
                loadChunk(map[x,y], center - x, center - y);
            }
        }
    }

    void loadChunk(Chunk c, int x, int y)
    {
        GameObject temp = GameObject.Instantiate(mapPiece);
        temp.transform.position = new Vector3(x * (chunkSize * 2), 0, y * (chunkSize * 2));
        temp.GetComponent<MeshGen>().loadChunk(c, x, y); 
    }
}
