using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public Dictionary<int, GameObject> playerList = new Dictionary<int, GameObject>();
    public GameObject[] playerOnlinePrefabs;    
    public GameObject[] playerLocalPrefabs;
    public Dictionary<int, GameObject> AIListDictionary = new Dictionary<int, GameObject>();
    public GameObject playerPrefab;    
    public GameObject playerHolder;
    public int playerID;
    public string ServerIP = "192.168.148.121";
    public int ServerPort = 28015;
    public string playerName;
    public InputField nameField;
    public int charVal;
    public GameObject playerUI;
    public Chat chatScript;
    public bool isConnected;

    private void Awake()
    {
        isConnected = false;
        instance = this;
        playerUI.SetActive(false);
        chatScript = GetComponent<Chat>();
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
            playerUI.SetActive(true);

    }

    public void InstantiateLocalPlayer()
    {
        playerPrefab = Instantiate(playerLocalPrefabs[charVal]);
        DontDestroyOnLoad(playerPrefab);
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
    
    public void ForwardChat(string message)
    {
        DataSender.SendChat(message);
    }
    
    public void ForwardAIPos(string AIPosXYZ)
    {
        DataSender.SendAIPos(AIPosXYZ);
    }
    public void ForwardEnemyState(string enemyState)
    {
        DataSender.SendEnemyState(enemyState);
    }

    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }
}
