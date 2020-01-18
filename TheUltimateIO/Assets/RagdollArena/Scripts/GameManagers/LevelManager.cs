using Character;
using Leaderboard;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using GameUI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public bool offlineMode;
    private bool ShowIfOffline() => offlineMode;
    [ShowIf("ShowIfOffline", true, true)]
    [SerializeField] private GameObject _myCamera;
    private LeaderboardManager _leaderboardMng;
    public LayerMask playerFriendsLayermask;
    [Tooltip("Posiciones random de spawneo")] public GameObject pointsSpawn;
    SpawnPoint[] _points;

    public bool finishLevel;
    public float pointsToWin;
    public GameObject panelWin;
    public TextMeshProUGUI[] nameWinner;

    [HideInInspector] public GameCanvas gameCanvas;

    public void DestroyAllInitialSpawnPoints()
    {
        //Eliminar todos los spawnpoints
        for (int i = 0; i < _points.Length; i++)
            Destroy(_points[i].gameObject);
        _points = new SpawnPoint[0];
    }
    public void RespawnRandom(Transform player)
    {
        player.position = PositionRandom();
        UpdateUserPoints(PhotonNetwork.NickName, -10);
    }
    Vector3 PositionRandom()
    {
        var selectRandom = Random.Range(0, _points.Length);
        return _points[selectRandom].transform.position;
    }
    public void UpdateUserPoints(string nickName, int addedPoints)
    { /*photonView.RPC("RPCUpdateUserPoints", RpcTarget.MasterClient, nickName, addedPoints);*/ }

    [PunRPC]
    private void RPCUpdateUserPoints(string newNickname, int addedPoints)
    { _leaderboardMng.UpdateUserPoints(newNickname, addedPoints); }

    public void UpdateLeaderboardTables(string[] names, int[] points)
    { /*photonView.RPC("RPCUpdateLeaderboardTables", RpcTarget.AllBuffered, names, points);*/ }

    [PunRPC]
    private void RPCUpdateLeaderboardTables(string[] names, int[] points)
    { _leaderboardMng.UpdateTableInfo(names, points); }
    
    [PunRPC]
    void FinishLevel(string top1, string top2, string top3)
    {
        /*finishLevel = true;
        panelWin.SetActive(true);

        nameWinner[0].text = top1;
        nameWinner[1].text = top2;
        nameWinner[2].text = top3;*/
    }

    public void Winner(string[] name)
    {
        var top1 = name.Length > 0 ? name[0] : "-";
        var top2 = name.Length > 1 ? name[1] : "-";
        var top3 = name.Length > 2 ? name[2] : "-";

        /*photonView.RPC("FinishLevel", RpcTarget.AllBuffered, top1, top2, top3);*/
    }

    public void BackMenu()
    {
        PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }

    private void Update()
    {
       /* if (!photonView.IsMine) return;*/

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

    private CharacterModel _spawnedModel;
    public CharacterModel SpawnUser()
    {
        var user = PhotonNetwork.Instantiate("User", PositionRandom(), Quaternion.identity);
        var model = user.GetComponentInChildren<CharacterModel>();
        _spawnedModel = model;
        return user.GetComponentInChildren<CharacterModel>();
    }

    private void Awake() //Al ser instanciado en realidad por el netmanager, no se va a llamar excepto q estemos testeando
    {
        gameCanvas = FindObjectOfType<GameCanvas>();
        Instance = this;

        _leaderboardMng = new LeaderboardManager(this);

        _points = FindObjectsOfType<SpawnPoint>();

        UpdateUserPoints(PhotonNetwork.NickName, 0);
    }
}
