using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Text connectionText;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private Camera sceneCamera;
    [SerializeField]
    private GameObject[] playerModel;
    [SerializeField]
    private GameObject serverWindow;
    [SerializeField]
    private GameObject messageWindow;
    [SerializeField]
    //private GameObject sightImage;
    //[SerializeField]
    private InputField username;
    [SerializeField]
    private InputField roomName;
    [SerializeField]
    private InputField roomList;
    [SerializeField]
    private InputField messagesLog;    
    private GameObject player;
    private Queue<string> messages;
    private const int messageCount = 10;
    private string nickNamePrefKey = "PlayerName";

    void Start()
    {
        //Sets username and keeps count of the messages sent
        messages = new Queue<string>(messageCount);
        if (PlayerPrefs.HasKey(nickNamePrefKey))
        {
            username.text = PlayerPrefs.GetString(nickNamePrefKey);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        connectionText.text = "Connecting to lobby...";
    }

    public override void OnConnectedToMaster()
    {
        //Connects to the main Photon Server
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Sets the connectionText label to the cause of the disconnect
        connectionText.text = cause.ToString();
    }

    public override void OnJoinedLobby()
    {
        //Displays the Lobby panel once connected to master
        serverWindow.SetActive(true);
        connectionText.text = "";
    }

    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        //refreshes the list of rooms
        roomList.text = "";
        foreach (RoomInfo room in rooms)
        {
            roomList.text += room.Name + "\n";
        }
    }

    public void JoinRoom()
    {
        //hides the lobby panel and jonis a room
        serverWindow.SetActive(false);
        connectionText.text = "Joining room...";
        PhotonNetwork.LocalPlayer.NickName = username.text;
        PlayerPrefs.SetString(nickNamePrefKey, username.text);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 5
        };
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinOrCreateRoom(roomName.text, roomOptions, TypedLobby.Default);
        }
        else
        {
            connectionText.text = "PhotonNetwork connection is not ready, try restart it.";
        }
    }

    public override void OnJoinedRoom()
    {
        connectionText.text = "";
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Respawn(0.0f);
    }

    void Respawn(float spawnTime)
    {
        //sightImage.SetActive(false);
        sceneCamera.enabled = true;
        StartCoroutine(RespawnCoroutine(spawnTime));
    }

    IEnumerator RespawnCoroutine(float spawnTime)
    {

        //Respawn Coroutine
        yield return new WaitForSeconds(spawnTime);
        messageWindow.SetActive(true);
        //sightImage.SetActive(true);
        int playerIndex = Random.Range(0, playerModel.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        player = PhotonNetwork.Instantiate(playerModel[playerIndex].name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation, 0);

        // Scripts to run to the network
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        PlugWire plugWire = player.GetComponent<PlugWire>();
        Grapple grapple = player.GetComponent<Grapple>();

        // Enabling networking on the scripts
        playerHealth.enabled = true;     
        plugWire.enabled = true;       
        grapple.enabled = true;

        playerHealth.RespawnEvent += Respawn;
        playerHealth.AddMessageEvent += AddMessage;
        
        sceneCamera.enabled = false;
        if (spawnTime == 0)
        {
            AddMessage("Player " + PhotonNetwork.LocalPlayer.NickName + " Joined Game.");
        }
        else
        {
            AddMessage("Player " + PhotonNetwork.LocalPlayer.NickName + " Respawned.");
        }
    }
    void AddMessage(string message)
    {
        photonView.RPC("AddMessage_RPC", RpcTarget.All, message);
    }

    [PunRPC]
    void AddMessage_RPC(string message)
    {
        //Simple message log, currently just in the console, soon to be a chat.
        Debug.Log("Message log : " + message);
        messages.Enqueue(message);
        if (messages.Count > messageCount)
        {
            messages.Dequeue();
        }
        messagesLog.text = "";
        foreach (string m in messages)
        {
            messagesLog.text += m + "\n";
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AddMessage("Player " + other.NickName + " Left Game.");
        }
    }

}
