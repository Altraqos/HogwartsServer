using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> playerType = new Dictionary<int, GameObject>();
    public GameObject[] playerOnlinePrefabs;    
    public GameObject[] playerLocalPrefabs;    
    public GameObject playerPrefab;    
    public GameObject playerHolder;
    public int playerID;

    public string playerName;
    public InputField nameField;

    public int charVal;

    private void Awake()
    {
        instance = this;     
        for (int i = 0; i < playerOnlinePrefabs.Length; i++)
        {
            playerType.Add(i, playerOnlinePrefabs[i]);
        }
    }

    public void chooseUserName()
    {
        playerName = nameField.text;
    }

    public void loadScene(int sceneToLoad)
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
        ClientHandleData.InitializePackets();
        ClientTCP.InitializingNetworking();
        InstantiateLocalPlayer();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void InstantiateLocalPlayer()
    {
        GameObject go = Instantiate(playerLocalPrefabs[charVal]);
        DontDestroyOnLoad(go);
    }

    public void chooseChar(int charValBTN)
    {
        charVal = charValBTN;
    }

    public void InstantiatePlayer(int index, int characterVal)
    {
        GameObject go = Instantiate(playerOnlinePrefabs[characterVal]);
        playerHolder = go;
        go.name = "player - {" + index + " - " + characterVal + "}";
        playerList.Add(index, go);
    }
    
    public void DestroyPlayer(int index)
    {
        GameObject go = playerList[index];
        Destroy(go);
        playerList.Remove(index);
    }

    public void ForwardPlayerPos(string playerPosXYZ)
    {
        DataSender.SendPlayerPos(playerPosXYZ);
    }

    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }
}
