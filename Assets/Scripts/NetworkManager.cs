using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Camera sceneCamera;
    private Camera m_Camera;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        Spawn();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room.");
        PhotonNetwork.CreateRoom("New Room", new Photon.Realtime.RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 5 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room with name exists.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Nice.");
    }

    public void Spawn()
    {
        GameObject player = (GameObject)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        player.GetComponentInChildren<CharacterController>().enabled = true;
        player.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        player.transform.Find("Camera").gameObject.SetActive(true);
        player.GetComponentInChildren<AudioListener>().enabled = true;
        sceneCamera.enabled = false;
    }
}
