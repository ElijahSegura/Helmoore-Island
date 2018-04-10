using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    NetworkManager nm;
    private HostData[] hostList;
    void Start()
    {
        nm = GetComponent<NetworkManager>();
        nm.StartHost();
        nm.StartClient();
    }
}
