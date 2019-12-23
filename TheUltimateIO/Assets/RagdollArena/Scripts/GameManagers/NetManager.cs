using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class NetManager : MonoBehaviourPunCallbacks
{
    public string nickname = "GenericNickname";
    public string gameVersion = "0.0.1";
    public int sceneID;

    public int minPlayersStartGame;
    public GameObject canvasWaitingStart;
    public TextMeshProUGUI textNamesPlayers, textWaiting;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.GameVersion = gameVersion;
        textWaiting.text = "There must be at least " + minPlayersStartGame + " players to start the game...";
    }

    public void Connect(int newSceneID)
    {
        sceneID = newSceneID;
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

        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) //es decir que somos el que creo el room
        {
             var go = PhotonNetwork.Instantiate("LevelManager", transform.position, Quaternion.identity);
             DontDestroyOnLoad(go);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersStartGame)
            StartCoroutine(WaitingStart());
        else
        {
            PhotonNetwork.LoadLevel(sceneID);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.LogWarning("OnSceneLoaded: " + scene.name);
        FindObjectOfType<LevelManager>().ArtificialAwake();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(returnCode + ": " + message);
        PhotonNetwork.CreateRoom(Random.Range(0, 9999).ToString(), new RoomOptions() { MaxPlayers = 19 });
    }

    IEnumerator WaitingStart()
    {
        var waitForEndOfFrame = new WaitForEndOfFrame();
        canvasWaitingStart.SetActive(true);
        while (PhotonNetwork.CurrentRoom.PlayerCount < minPlayersStartGame)
        {
            Debug.Log("Waiting players...");
            yield return waitForEndOfFrame;
        }
        canvasWaitingStart.SetActive(false);
        PhotonNetwork.LoadLevel(sceneID);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    /*
    [PunRPC]
    void AddNameWaiting(string name)
    {
        textNamesPlayers.text += " - " + name;
    }

    [PunRPC]
    void FirstAddNameWaiting(string name)
    {
        textNamesPlayers.text += name;
    }
    */
}
