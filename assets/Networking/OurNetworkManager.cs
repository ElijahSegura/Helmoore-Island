using assets.Map;
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
    //public class MyMsgType
    //{
    //    public static short GetMap = MsgType.Highest + 1;
    //};

    //public class NetworkMessage_MapData : MessageBase
    //{
    //    public Row[] serializedMapHeights;
    //    public int mapSize;
    //    public int chunkSize;
    //    public int mapWH;
    //    public float chunkScale;
    //}
    
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
        public GameObject playerPrefab;
        NetworkClient myClient;

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

            GUILayout.Label("STATE: " + NetworkServer.active);
            
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
            //Square[,] map = mapData.GetHighPolyMap();
            //NetworkMessage_MapData msg = new NetworkMessage_MapData(mapData.MapHeights, mapData.mapSize, mapData.chunkSize, mapData.mapWH);
            List<Row> serializedMapHeights = new List<Row>();
            int size = (int)newMap.GetLongLength(0) * sizeof(float);
            for (int x = 0; x < newMap.GetLongLength(0); x++)
            {
                float[] target = new float[newMap.GetLongLength(1)];
                Row newRow = new Row() { data = new float[newMap.GetLongLength(1)] };
                Buffer.BlockCopy(newMap, size * x, newRow.data, 0, size);
                serializedMapHeights.Add(newRow);
            }

            NetworkMessage_MapData msg = new NetworkMessage_MapData() { serializedMapHeights = serializedMapHeights.ToArray(), chunkSize = mapData.chunkSize, mapSize = mapData.mapSize, mapWH = mapData.mapWH, chunkScale = mapData.chunkscale };

            NetworkServer.SendToClient(
                message.conn.connectionId,
                MyMsgType.GetMap,
                msg
                );



            //GameObject thePlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            //NetworkServer.AddPlayerForConnection(message.conn, thePlayer, (short)message.conn.connectionId);
            //NetworkServer.AddPlayerForConnection(message.conn, Instantiate(Resources.Load("Player/Player_1"), new Vector3(0, 10, 0), Quaternion.identity) as GameObject, (short)message.conn.connectionId);
        }
        

        
        DualMapTest mapData;
        float[,] newMap;
        public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Create match succeeded");
                matchCreated = true;

                try
                {
                    mapData = new DualMapTest() { mat = mat };
                    newMap = mapData.CreateNewMapHeights();
                    Debug.Log("Map Generated");
                    message = "Map Generated";
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                    message = e.Message;    
                }


                NetworkServer.Listen(matchInfo, 9000);
                NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnect);
                NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayer);
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
                
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
                myClient = new NetworkClient();
                myClient.RegisterHandler(MsgType.Connect, OnConnected);
                myClient.RegisterHandler(MyMsgType.GetMap, HandleNewMap);
                myClient.Connect(matchInfo);
                ClientScene.RegisterPrefab(playerPrefab);

            }
            else
            {
                Debug.LogError("Join match failed " + extendedInfo);
            }
        }
        

        private void OnAddPlayer(NetworkMessage netMsg)
        {
            GameObject thePlayer = (GameObject)Instantiate(playerPrefab, new Vector3(0, 1000, 0), Quaternion.identity);
            //NetworkServer.AddPlayerForConnection(networkMessage.conn, thePlayer, 0);
            bool result = NetworkServer.AddPlayerForConnection(netMsg.conn, thePlayer, 0);
        }

        private void HandleNewMap(NetworkMessage netMsg)
        {
            try
            {
                if(netMsg == null)
                {
                    Debug.Log("netMsg is null");
                }
                message = "Receiving map";
                NetworkMessage_MapData msg = netMsg.ReadMessage<NetworkMessage_MapData>();
                float[,] MapHeights = new float[msg.serializedMapHeights[0].data.Length, msg.serializedMapHeights.Count()];

                Debug.Log(msg.mapWH);

                int size = (int)MapHeights.GetLongLength(0) * sizeof(float);

                for (int x = 0; x < msg.serializedMapHeights.Length; x++)
                {
                    Row row = msg.serializedMapHeights[x];
                    Buffer.BlockCopy(row.data, 0, MapHeights, size * x, size);
                }


                RenderMap(msg.mapSize, msg.chunkSize, msg.chunkScale, MapHeights, mat);

                //ClientScene.AddPlayer(netMsg.conn,(short)netMsg.conn.connectionId);
                myClient.Send(MsgType.AddPlayer, new EmptyMessage());
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }

        }
        
        void RenderMap(int mapSize, int chunkSize, float chunkscale, float[,] MapHeights, Material material)
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    Chunk c = new Chunk(chunkSize, mapSize, x, y, chunkscale, MapHeights);
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
                        temp.GetComponent<MeshRenderer>().material = material;
                        temp.transform.position = new Vector3(((x * (chunkSize * chunkscale)) - (50000 / 2)), 0, ((y * (chunkSize * chunkscale)) - (50000 / 2)));
                    }
                }
            }
        }



        public void OnConnected(NetworkMessage msg)
        {
            Debug.Log("Connected!");
            message = "Connected!";

            //Instantiate(Resources.Load("Player/Player_1"), new Vector3(0, 10, 0), Quaternion.identity);
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


        
        //public override void OnClientConnect(NetworkConnection conn){Debug.Log("OnClientConnect");}
        //public override void OnClientDisconnect(NetworkConnection conn){Debug.Log("OnClientDisconnect");}
        //public override void OnClientError(NetworkConnection conn, int errorCode){Debug.Log("OnClientError");}
        //public override void OnClientNotReady(NetworkConnection conn){Debug.Log("OnClientNotReady");}
        //public override void OnClientSceneChanged(NetworkConnection conn){Debug.Log("OnClientSceneChanged");}
        //public override void OnDestroyMatch(bool success, string extendedInfo){Debug.Log("OnDestroyMatch");}
        //public override void OnDropConnection(bool success, string extendedInfo){Debug.Log("OnDropConnection");}
        //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader){Debug.Log("OnServerAddPlayer");}
        //public override void OnServerConnect(NetworkConnection conn){Debug.Log("OnServerConnect");}
        //public override void OnServerDisconnect(NetworkConnection conn){Debug.Log("OnServerDisconnect");}
        //public override void OnServerError(NetworkConnection conn, int errorCode){Debug.Log("OnServerError");}
        //public override void OnServerReady(NetworkConnection conn){Debug.Log("OnServerReady");}
        //public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player){Debug.Log("OnServerRemovePlayer");}
        //public override void OnServerSceneChanged(string sceneName){Debug.Log("OnServerSceneChanged");}
        //public override void OnSetMatchAttributes(bool success, string extendedInfo){Debug.Log("OnSetMatchAttributes");}
        //public override void OnStartClient(NetworkClient client){Debug.Log("OnStartClient");}
        //public override void OnStartHost(){Debug.Log("OnStartHost");}
        //public override void OnStartServer(){Debug.Log("OnStartServer");}
        //public override void OnStopClient(){Debug.Log("OnStopClient");}
        //public override void OnStopHost(){Debug.Log("OnStopHost");}
        //public override void OnStopServer(){Debug.Log("OnStopServer");}

        


        //public static void RegisterStartPosition(Transform start){Debug.Log("MESSAGE");}
        //public static void Shutdown(){Debug.Log("MESSAGE");}
        //public static void UnRegisterStartPosition(Transform start){Debug.Log("MESSAGE");}
        //public Transform GetStartPosition(){Debug.Log("MESSAGE");}
        //public bool IsClientConnected(){Debug.Log("MESSAGE");}
        //public override void ServerChangeScene(string newSceneName){Debug.Log("ServerChangeScene");}
        //public void SetMatchHost(string newHost, int port, bool https){Debug.Log("MESSAGE");}
        //public void SetupMigrationManager(NetworkMigrationManager man){Debug.Log("MESSAGE");}
        //public NetworkClient StartClient(){Debug.Log("MESSAGE");}
        //public NetworkClient StartClient(MatchInfo matchInfo){Debug.Log("MESSAGE");}
        //public NetworkClient StartClient(MatchInfo info, ConnectionConfig config){Debug.Log("MESSAGE");}
        //public override NetworkClient StartHost(){Debug.Log("StartHost blank"); return null; }
        //public override NetworkClient StartHost(MatchInfo info){Debug.Log("StartHost matchinfo"); return null; }
        //public override NetworkClient StartHost(ConnectionConfig config, int maxConnections){Debug.Log("StartHost connectionconfig"); return null; }
        //public void StartMatchMaker(){Debug.Log("MESSAGE");}
        //public bool StartServer(){Debug.Log("MESSAGE");}
        //public bool StartServer(MatchInfo info){Debug.Log("MESSAGE");}
        //public bool StartServer(ConnectionConfig config, int maxConnections){Debug.Log("MESSAGE");}
        //public void StopClient(){Debug.Log("MESSAGE");}
        //public void StopHost(){Debug.Log("MESSAGE");}
        //public void StopMatchMaker(){Debug.Log("MESSAGE");}
        //public void StopServer(){Debug.Log("MESSAGE");}
        //public void UseExternalClient(NetworkClient externalClient){Debug.Log("MESSAGE");}
    }
}
