using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    void Connect()
    {
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
        GameObject currentPlayer = (GameObject)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        currentPlayer.GetComponentInChildren<CharacterController>().enabled = true;
        currentPlayer.GetComponentInChildren<PlayerMove>().enabled = true;

        GameObject currCam = currentPlayer.transform.Find("PlayerCamera").gameObject;
        currCam.SetActive(true);
        currCam.GetComponentInChildren<Camera>().enabled = true;
        currCam.GetComponentInChildren<PlayerLook>().enabled = true;
    }
}
