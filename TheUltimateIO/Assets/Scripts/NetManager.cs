using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " has been connected to server!");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has entered the lobby!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has entered the room :" + PhotonNetwork.CurrentRoom + "!");
        PhotonNetwork.LoadLevel("MainScene");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(returnCode + ": " + message);
        PhotonNetwork.CreateRoom(Random.Range(0, 9999).ToString(), new RoomOptions() { MaxPlayers = 19 });
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(PhotonNetwork.LocalPlayer.NickName + 
            " has been disconnected from server for reason: " + cause.ToString());
    }
}
