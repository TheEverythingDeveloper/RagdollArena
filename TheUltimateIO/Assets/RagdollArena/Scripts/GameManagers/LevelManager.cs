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
    public LayerMask playerFriendsLayermask;
    [Tooltip("Posiciones random de spawneo")] public GameObject pointsSpawn;
    SpawnPoint[] _points;

    public int amountOfBricksPerSecond;
    public bool finishLevel;
    public float pointsToWin;
    public GameObject panelWin;
    public TextMeshProUGUI[] nameWinner;

    [HideInInspector] public GameCanvas gameCanvas;

    public void DestroyAllInitialSpawnPoints() //Eliminar todos los spawnpoints
    {
        for (int i = 0; i < _points.Length; i++)
            Destroy(_points[i].gameObject);
        _points = new SpawnPoint[0];
    }
    public Vector3 PrecisionPositionRandom()
    {
        float rnd = Random.Range(-transform.position.z, transform.position.z);
        float rnd2 = Random.Range(-transform.position.z, transform.position.z);
        return new Vector3(rnd *2, 0, rnd2 *2);
    }
    public void RespawnRandom(Transform player) { player.position = PositionRandom(); }
    public Vector3 PositionRandom()
    {
        var selectRandom = Random.Range(0, _points.Length);
        return _points[selectRandom].transform.position;
    }

    public void BackMenu()
    {
        PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }

    public CharacterModel SpawnUser()
    {
        var user = PhotonNetwork.Instantiate("User", PositionRandom(), Quaternion.identity);
        var model = user.GetComponentInChildren<CharacterModel>();
        return user.GetComponentInChildren<CharacterModel>();
    }

    private void Awake() //Al ser instanciado en realidad por el netmanager, no se va a llamar excepto q estemos testeando
    {
        gameCanvas = FindObjectOfType<GameCanvas>();
        Instance = this;
        SetInitialSpawnPoints();
    }

    public void SetInitialSpawnPoints() { _points = FindObjectsOfType<SpawnPoint>(); }
}
