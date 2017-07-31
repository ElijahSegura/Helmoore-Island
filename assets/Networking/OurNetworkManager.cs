using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

namespace assets.Networking
{
    public class MyMsgType
    {
        public static short GetMap = MsgType.Highest + 1;
    };

    public class NetworkMessage_MapData : MessageBase
    {
        public Square[,] MapData { get; private set; }

        public int mapSize;
        public int chunkSize;
        public float[,] vertZ;
        public float chunkScale;

        public NetworkMessage_MapData(Square[,] MapData, int mapSize, int chunkSize, float[,] vertZ, float chunkScale)
        {
            this.MapData = MapData;
            this.mapSize = mapSize;
            this.chunkSize = chunkSize;
            this.vertZ = vertZ;
            this.chunkScale = chunkScale;
        }
        public NetworkMessage_MapData()
        {

        }
    }
    
    class OurNetworkManager : MonoBehaviour
    {
        //private void Update()
        //{
        //    if (NetworkServer.active)
        //    {
                
        //        if (Input.GetKeyDown(KeyCode.S))
        //        {
        //            NetworkServer.SendToAll(MyMsgType.GetMap, new TestMessage() { message = "Did it work?" });
        //        }
        //    }
        //}

        public Material mat;

        List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
        bool matchCreated;
        NetworkMatch networkMatch;

        //https://docs.unity3d.com/Manual/UnityMultiplayerIntegratingHighLevel.html

        void Awake()
        {
            networkMatch = gameObject.AddComponent<NetworkMatch>();
        }
        
        string message = "nothing for now";

        void OnGUI()
        {
            // You would normally not join a match you created yourself but this is possible here for demonstration purposes.
            if (GUILayout.Button("Create Room"))
            {
                string matchName = "room";
                uint matchSize = 4;
                bool matchAdvertise = true;
                string matchPassword = "";

                networkMatch.CreateMatch(matchName, matchSize, matchAdvertise, matchPassword, "", "", 0, 0, OnMatchCreate);
            }

            if (GUILayout.Button("List rooms"))
            {
                networkMatch.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
            }
            

            if(matchCreated)
            {
                GUILayout.Label("Connections " + NetworkServer.connections.Count);

                if(GUILayout.Button("Shutdown Server"))
                {
                    NetworkServer.Shutdown();
                }
            }

            if (matchList.Count > 0)
            {
                GUILayout.Label("Current rooms");
            }
            foreach (var match in matchList)
            {
                if (GUILayout.Button(match.name))
                {
                    networkMatch.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
                }
            }

            GUILayout.Label(message);
            
        }

        public void OnPlayerConnect(NetworkMessage message)
        {
            NetworkServer.SendToClient(
                message.conn.connectionId, 
                MyMsgType.GetMap, 
                new NetworkMessage_MapData(mapData.GetHighPolyMap(), 
                    mapData.mapSize,
                    mapData.chunkSize,
                    mapData.vertZ,
                    mapData.chunkscale)
                );

        }
        
        DualMapTest mapData;
        public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Create match succeeded");
                matchCreated = true;
                NetworkServer.Listen(matchInfo, 9000);
                NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnect);
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);

                try
                {
                    MapGen MapFirst = new MapGen() { Materials = new List<Material> { mat } };
                    MapGen MapSecond = new MapGen() { Materials = new List<Material> { mat } };
                    mapData = new DualMapTest() { MapA = MapFirst, MapB = MapSecond };
                    mapData.Start();
                    Debug.Log("Map Generated");
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }

                //SceneManager.LoadSceneAsync("GameMap", LoadSceneMode.Additive);

                //GameObject[] goArray = SceneManager.GetSceneByName("GameMap").GetRootGameObjects();
                //if (goArray.Length > 0)
                //{
                //    for (int i = 0; i < goArray.Length; i++)
                //    {
                //        GameObject rootGo = goArray[i];
                //        Debug.Log(rootGo.name);
                //    }

                //    // Do something with rootGo here...
                //}
            }
            else
            {
                Debug.LogError("Create match failed: " + extendedInfo);
            }
        }






        public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success && matches != null && matches.Count > 0)
            {
                matchList = matches;
                //networkMatch.JoinMatch(matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);
            }
            else if (!success)
            {
                Debug.LogError("List match failed: " + extendedInfo);
            }
        }

        public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Join match succeeded");
                if (matchCreated)
                {
                    Debug.LogWarning("Match already set up, aborting...");
                    return;
                }
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
                NetworkClient myClient = new NetworkClient();
                myClient.RegisterHandler(MsgType.Connect, OnConnected);
                myClient.RegisterHandler(MyMsgType.GetMap, HandleNewMap);
                myClient.Connect(matchInfo);
            }
            else
            {
                Debug.LogError("Join match failed " + extendedInfo);
            }
        }

        private void HandleNewMap(NetworkMessage netMsg)
        {
            try
            {
                if(netMsg == null)
                {
                    Debug.Log("netMsg is null");
                }
                NetworkMessage_MapData msg = netMsg.ReadMessage<NetworkMessage_MapData>();
                Debug.Log(String.Format("Map data Received {0},{1},{2},{3},{4}", msg.mapSize, msg.chunkScale, msg.MapData.ToString(), msg.vertZ));
                RenderMap(msg.MapData, msg.mapSize, msg.chunkSize, msg.vertZ, msg.chunkScale);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }

        }

        Chunk[,] chunkMap;
        void RenderMap(Square[,] HighPoly,int mapSize, int chunkSize, float[,] vertZ, float chunkscale)
        {
            chunkMap = new Chunk[mapSize, mapSize];
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, vertZ, HighPoly);
                    if (!c.isflat())
                    {
                        GameObject temp = new GameObject(x + ":" + y);
                        temp.AddComponent<MeshRenderer>();
                        temp.GetComponent<MeshRenderer>().receiveShadows = true;
                        temp.AddComponent<MeshFilter>();
                        temp.GetComponent<MeshFilter>().mesh = c.getMesh();
                        temp.AddComponent<MeshCollider>();
                        temp.GetComponent<MeshCollider>().sharedMesh = c.getMesh();
                        temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
                        temp.GetComponent<MeshRenderer>().material = mat;
                        temp.transform.position = new Vector3(((x * (chunkSize * chunkscale)) - (50000 / 2)), 0, ((y * (chunkSize * chunkscale)) - (50000 / 2)));
                    }
                }
            }
        }

        public void OnConnected(NetworkMessage msg)
        {
            Debug.Log("Connected!");
            message = "Connected!";
        }


        //// Create a server and listen on a port
        //public void SetupServer()
        //{
        //    NetworkServer.Listen(4444);
        //    startUp = false;
        //}

        //NetworkClient myClient;

        //// Create a client and connect to the server port
        //public void SetupClient()
        //{
        //    myClient = new NetworkClient();
        //    myClient.RegisterHandler(MsgType.Connect, OnConnected);
        //    myClient.Connect("127.0.0.1", 4444);
        //    startUp = false;
        //}

        //public void OnConnected(NetworkMessage netMsg)
        //{
        //    Debug.Log("Connected to server");
        //}

        //public override void OnStartServer()
        //{
        //    mapGeneration = new MapGen();
        //    mapGeneration.StartMapGen();
        //    map = mapGeneration.sharableChunks;
        //    //DEALLOCATE MAPGENERATION OBJECT
        //    foreach(Chunk c in map)
        //    {
        //        if (!c.isflat())
        //        {
        //            GameObject temp = new GameObject("Chunk: " + c.posx + ":" + c.posy);
        //            temp.AddComponent<MeshRenderer>();
        //            temp.GetComponent<MeshRenderer>().receiveShadows = true;
        //            temp.AddComponent<MeshFilter>();
        //            temp.GetComponent<MeshFilter>().mesh = c.getMesh();
        //            temp.AddComponent<MeshCollider>();
        //            temp.GetComponent<MeshCollider>().sharedMesh = c.getMesh();
        //            temp.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //            temp.GetComponent<MeshRenderer>().material = mapGeneration.Materials[0];
        //            temp.transform.position = new Vector3(((c.posx * (mapGeneration.chunkSize * mapGeneration.chunkscale)) - (mapGeneration.mapWH / 2)), 0, ((c.posy * (mapGeneration.chunkSize * mapGeneration.chunkscale)) - (mapGeneration.mapWH / 2)));
        //        }
        //    }

        //}
    }
}
