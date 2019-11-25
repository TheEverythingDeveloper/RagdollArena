using Character;
using Gamemodes;
using Leaderboard;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class LevelManager : MonoBehaviourPun
{
    public bool offlineMode;
    private bool ShowIfOffline() => offlineMode;
    [ShowIf("ShowIfOffline", true, true)]
    [SerializeField] private GameObject _myCamera;
    private LeaderboardManager _leaderboardMng;
    public LayerMask playerFriendsLayermask;
    [Tooltip("Posiciones random de spawneo")]
    public GameObject pointsSpawn;
    Transform[] _points;

    public bool finishLevel;
    public float pointsToWin;
    public GameObject panelWin;
    public TextMeshProUGUI nameWinner;

    public void ArtificialAwake()
    {
        Debug.Log("Starting Level Manager");
        pointsSpawn = GameObject.Find("AllSpawnPoint");
        _points = pointsSpawn.GetComponentsInChildren<Transform>();
        var user = PhotonNetwork.Instantiate("User",
            PositionRandom(), Quaternion.identity);
        user.GetComponentInChildren<CharacterModel>().name = PhotonNetwork.NickName;
        user.GetComponentInChildren<Character3DUI>().photonView.RPC("RPCUpdateNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
        //GGM.Instance.user = user.GetComponentInChildren<CharacterModel>();

        _leaderboardMng = new LeaderboardManager(this);

        if (photonView.IsMine)
        {
            _leaderboardMng.table = FindObjectOfType<LeaderboardTable>();
            StartCoroutine(_leaderboardMng.InactivePlayersCoroutine());
        }

        UpdateUserPoints(PhotonNetwork.NickName, 0);
    }

    public void RespawnRandom(Transform player)
    {
        player.position = PositionRandom();
        UpdateUserPoints(PhotonNetwork.NickName, -10);
    }
    Vector3 PositionRandom()
    {
        var selectRandom = Random.Range(0, _points.Length);
        return _points[selectRandom].position;
    }
    public void UpdateUserPoints(string nickName, int addedPoints)
    { photonView.RPC("RPCUpdateUserPoints", RpcTarget.MasterClient, nickName, addedPoints); }

    [PunRPC]
    private void RPCUpdateUserPoints(string newNickname, int addedPoints)
    { _leaderboardMng.UpdateUserPoints(newNickname, addedPoints); }

    public void UpdateLeaderboardTables(string[] names, int[] points)
    { photonView.RPC("RPCUpdateLeaderboardTables", RpcTarget.AllBuffered, names, points); }

    [PunRPC]
    private void RPCUpdateLeaderboardTables(string[] names, int[] points)
    { _leaderboardMng.UpdateTableInfo(names, points); }

    private GamemodeType _actualGMType;
    public void ChangeGMType(GamemodeType newType)
    { photonView.RPC("RPCChangeGMType", RpcTarget.MasterClient, newType); }

    [PunRPC]
    private void RPCChangeGMType(GamemodeType newType)
    { 
        _actualGMType = newType;
    }

    public void NewGM(GamemodeType gameModeTypeID)
    {
        int ID = (int)gameModeTypeID;
        photonView.RPC("RPCNewGameMode", RpcTarget.AllBuffered, ID);
    }

    [PunRPC]
    private void RPCNewGameMode(int gameModeTypeID)
    {
        GGM.Instance.StartGameMode(gameModeTypeID);
    }

    public void UpdateGM(params object[] allParams)
    { photonView.RPC("RPCUpdateGM", RpcTarget.All, allParams); }

    [PunRPC]
    public void RPCUpdateGM(params object[] allParams)
    {
        switch (_actualGMType)
        {
            case GamemodeType.None:
                break;
            case GamemodeType.FriendzoneGM:
                break;
            default:
                break;
        }
    }

    [PunRPC]
    void FinishLevel(string name)
    {
        finishLevel = true;
        panelWin.SetActive(true);
        nameWinner.text = name;
    }

    public void Winner(string name)
    {
        photonView.RPC("FinishLevel", RpcTarget.AllBuffered, name);
    }


    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.R)) //debugear el top del ranking en orden de mayor a menor
            _leaderboardMng.DebugTopRanking();

        if (Input.GetKeyDown(KeyCode.N)) //agregar un nuevo "jugador" para el leaderboard
        {
            UpdateUserPoints("randomname" + Random.Range(0, 1000), Random.Range(0, 1000));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) //shuffle a todo el leaderboard, cambian todos los puntos de todos
        {
            _leaderboardMng.ShufflePoints();
            _leaderboardMng.DebugTopRanking();
        }
    }

    private void Awake() //Al ser instanciado en realidad por el netmanager, no se va a llamar excepto q estemos testeando
    {
        PhotonNetwork.OfflineMode = offlineMode;

        if (PhotonNetwork.OfflineMode)
        {
            Instantiate(_myCamera, transform.position, transform.localRotation);
        }
        else
        {
            if (photonView.IsMine) return;
        }

        ArtificialAwake();

        _leaderboardMng.table = FindObjectOfType<LeaderboardTable>();
    }
}
