using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManager : MonoBehaviourPunCallbacks
{
    public string nickname = "GenericNickname";
    public string gameVersion = "0.0.1";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.GameVersion = gameVersion;
    }

    public void Connect()
    {
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
        _lvlmng = FindObjectOfType<LevelManager>();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has entered the room :" + PhotonNetwork.CurrentRoom + "!");
        PhotonNetwork.LoadLevel("MainScene");
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1) //es decir que somos el que creo el room
        {
            var go = PhotonNetwork.Instantiate("LevelManager", transform.position, Quaternion.identity);
            DontDestroyOnLoad(go);
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

    private LevelManager _lvlmng;

    public override void OnLeftRoom()
    {
        //_lvlmng.RemoveUserLeaderboard();
        //_lvlmng.UpdateUserPoints(PhotonNetwork.NickName, Leaderboard.LeaderboardManager.REMOVE); //desconectarse del ranking
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(PhotonNetwork.LocalPlayer.NickName +
            " has been disconnected from server for reason: " + cause.ToString());

        _lvlmng.RemoveUserLeaderboard();
        //_lvlmng.UpdateUserPoints(PhotonNetwork.NickName, Leaderboard.LeaderboardManager.REMOVE); //desconectarse del ranking
    }
}
