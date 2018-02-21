using assets.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace assets.Networking
{

    public class MyMsgType
    {
        public static short GetMap = MsgType.Highest + 1;
    };

    public class NetworkMessage_MapData : MessageBase
    {
        public Row[] serializedMapHeights;
        public int mapSize;
        public int chunkSize;
        public int mapWH;
        public float chunkScale;
    }

    public class OverrideNetworkManager : NetworkManager
    {
        private DualMapTest mapData;
        private float[,] newMap;
        public Material mat;

        public override void OnStartServer()
        {
            mapData = new DualMapTest() { mat = mat };
            newMap = mapData.CreateNewMapHeights();
            Debug.Log("Map Generated");

            base.OnStartServer();
        }
        //public override void OnServerConnect(NetworkConnection conn)
        //{
        //    mapData = new DualMapTest() { mat = mat };
        //    newMap = mapData.CreateNewMapHeights();
        //    Debug.Log("Map Generated");

        //    //NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnect);
        //    NetworkServer.RegisterHandler(MsgType.AddPlayer, OnAddPlayer);

        //    base.OnServerConnect(conn);
        //}

        public override void OnClientConnect(NetworkConnection conn)
        {
            conn.RegisterHandler(MyMsgType.GetMap, HandleNewMap);
            base.OnClientConnect(conn);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
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
                conn.connectionId,
                MyMsgType.GetMap,
                msg
                );

            base.OnServerConnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            base.OnServerAddPlayer(conn, playerControllerId);
        }

        private void HandleNewMap(NetworkMessage netMsg)
        {
            try
            {
                if (netMsg == null)
                {
                    Debug.Log("netMsg is null");
                }
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
            }
            catch (Exception e)
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
    }

}
