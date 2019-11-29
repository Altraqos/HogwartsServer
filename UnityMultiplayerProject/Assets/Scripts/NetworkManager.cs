using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
    public GameObject playerPref;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();

        ClientHandleData.InitializePackets();
        ClientTCP.InitializingNetworking();
    }

    public void InstantiatePlayer(int index)
    {
        GameObject go = Instantiate(playerPref);
        go.name = ("player: " + index);
        playerList.Add(index, go);
    }

    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }
}
