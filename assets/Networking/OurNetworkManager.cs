using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace assets.Networking
{
    public class MyMsgType
    {
        public static short TestCommand = MsgType.Highest + 1;
    };

    class OurNetworkManager : MonoBehaviour
    {
        MapGen mapGeneration;
        List<Chunk> map;

        private Boolean startUp = true;

        private void Update()
        {
            if (matchCreated)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    NetworkServer.SendToAll(MyMsgType.TestCommand, new TestMessage() { message = "Did it work?" });
                }
            }
        }

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

            //if (startUp)
            //{
            //    GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            //    GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");
            //    GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
            //}
        }
        

        public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            if (success)
            {
                Debug.Log("Create match succeeded");
                matchCreated = true;
                NetworkServer.Listen(matchInfo, 9000);
                Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
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
                myClient.RegisterHandler(MyMsgType.TestCommand, testRecieveCommand);
                myClient.Connect(matchInfo);
            }
            else
            {
                Debug.LogError("Join match failed " + extendedInfo);
            }
        }

        private void testRecieveCommand(NetworkMessage netMsg)
        {
            Debug.Log("RECEIVED!");
            TestMessage hehe = netMsg.ReadMessage<TestMessage>();
            Debug.Log(hehe.message);
            message = hehe.message;
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

    public class TestMessage : MessageBase
    {
        public string message;
    }

}
