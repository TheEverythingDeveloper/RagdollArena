using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManager : MonoBehaviourPunCallbacks
{
    public static NetManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string nickname = "GenericNickname";
    public string gameVersion = "0.0.1";
    public int sceneID;

    private bool _host;

    public int minPlayersStartGame; //cantidad de jugadores que tienen que haber entrado para que inicie el juego
    public GameObject canvasWaitingStart;
    public TextMeshProUGUI textNamesPlayers, textWaiting;

    Server server;

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        textWaiting.text = "There must be at least " + minPlayersStartGame + " players to start the game...";
    }

    public void Connect(int newSceneID) //Inicio de conectarse despues del lobby
    {
        sceneID = newSceneID;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " has been connected to server!");
        if (PhotonNetwork.CountOfPlayers <= 1)
            _host = true;

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has entered the lobby!");
        if (_host)
            PhotonNetwork.CreateRoom("MainRoom", new RoomOptions { MaxPlayers = 10 });
        else
            PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has entered the room :" + PhotonNetwork.CurrentRoom + "!");

        if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersStartGame)
            StartCoroutine(WaitingStart());
        else
            SceneLoad();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(returnCode + ": " + message);
        PhotonNetwork.CreateRoom(Random.Range(0, 9999).ToString(), new RoomOptions() { MaxPlayers = 19 });
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected because " + cause.ToString());
        //avisar que alguien se fue al server

        server.RemovePlayer(PhotonNetwork.LocalPlayer);
    }

    IEnumerator WaitingStart()
    {
        canvasWaitingStart.SetActive(true);
        while (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersStartGame)
        {
            Debug.Log("Waiting players...");
            yield return new WaitForEndOfFrame();
        }
        canvasWaitingStart.SetActive(false);
        SceneLoad();
    }

    private void SceneLoad()
    {
        PhotonNetwork.LoadLevel(sceneID);
        SceneManager.sceneLoaded += StartGameTransition;
    }

    private void StartGameTransition(Scene scene, LoadSceneMode mode)
    {
        if (_host)
            PhotonNetwork.Instantiate("Server", Vector3.zero, Quaternion.identity);
        else
            PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity);

        server = FindObjectOfType<Server>();
    }
}
