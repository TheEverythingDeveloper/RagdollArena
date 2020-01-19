using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using TMPro;

namespace GameUI
{
    public class SpawnMap : MonoBehaviourPun
    {
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _coordText;
        public Image[] _allPointers = new Image[4];
        [SerializeField] private Image _corePointer;
        private float _mapPointerPosScaler = 1.43f;
        [Tooltip("Literal es un multiplicador de cuanto va a ser el ancho y largo del mapa")] public float mapScale = 0.5f;

        public int playerID; //numero de jugador que es el player en el equipo
        public int teamID; //numero de equipo para saber color
        private Vector3 _spawnPointPos;

        private Dictionary<int, bool[]> _pointerConfirmationList = new Dictionary<int, bool[]>();
        private int _playersAmount;

        [SerializeField] private SpawnMapArea _spawnMapArea;

        [PunRPC] public void RPCUpdatePlayerIDs(int newPlayerID, int newTeamID) => UpdatePlayerIDs(newPlayerID, newTeamID);
        public void UpdatePlayerIDs(int newPlayerID, int newTeamID) //esto se llama en cada jugador, actualizando estos valores de la clase
        {
            Debug.Log("SE ACTUALIZO EL TEAM A " + newTeamID + " Y EL NEWPLAYERTEAM A " + newPlayerID);
            this.playerID = newPlayerID;
            teamID = newTeamID;
        }

        public void SetTeamAmountOfPlayers(int teamsAmount, int playersAmount) //esto se tiene que reproducir solo en el servidor
        {
            _pointerConfirmationList.Clear();
            _playersAmount = playersAmount;
            for (int i = 0; i < teamsAmount; i++)
                _pointerConfirmationList.Add(i, new bool[playersAmount]); //se van a agregar la cantidad de confirmaciones en false como players

            photonView.RPC("RPCSetPlayersAmount", RpcTarget.OthersBuffered, playersAmount);   
        }
        [PunRPC] private void RPCSetPlayersAmount(int playersAmount) => _playersAmount = playersAmount;
        [PunRPC] public void RPCUpdatePointer(int playerID, int team, Vector2 pos)
        {
            if (team != teamID) return;
            _allPointers[playerID].gameObject.SetActive(true);
            if(_allPointers[playerID].color != Color.white)
            {
                _allPointers[playerID].color = _corePointer.color = 
                    team == 0 ? Color.blue : team == 1 ? Color.red : team == 2 ? Color.yellow : Color.green;
            }
            _allPointers[playerID].rectTransform.anchoredPosition = pos;
            Vector2 totalPos = _allPointers.Aggregate(Vector2.zero, (x, y) => x + y.rectTransform.anchoredPosition);
            _corePointer.rectTransform.anchoredPosition = totalPos / _playersAmount;
        }

        public void PointerClick() { }

        public void PointerMove(float horAxis, float verAxis)
        {
            RectTransform rect = _allPointers[playerID].rectTransform;
            rect.anchoredPosition =
                new Vector2(rect.anchoredPosition.x + horAxis * 5, rect.anchoredPosition.y + verAxis * 5);

            rect.anchoredPosition = 
                new Vector2(Mathf.Clamp(rect.anchoredPosition.x, -150, 150), Mathf.Clamp(rect.anchoredPosition.y, -150, 150));

            _spawnPointPos = rect.anchoredPosition;

            photonView.RPC("RPCUpdatePointer", RpcTarget.All, playerID, teamID, rect.anchoredPosition);
        }

        public void SpawnButton()
        {
            photonView.RPC("RPCPlayerSpawnConfirmed", RpcTarget.All, playerID, teamID);
            photonView.RPC("RPCServerSpawnConfirmation", RpcTarget.MasterClient, playerID, teamID);
        }

        [PunRPC] public void RPCResetAllPointers()
        {
            foreach (var x in _allPointers)
            {
                x.rectTransform.anchoredPosition = Vector2.zero;
                x.color = Color.black;
            }
            _corePointer.rectTransform.anchoredPosition = Vector2.zero;
        }

        [PunRPC] public void RPCPlayerSpawnConfirmed(int playerID, int selectedTeamID) //esto se llama cuando se apreta el boton de spawn
        {
            if (selectedTeamID != teamID) return;

            _allPointers[playerID].color = Color.white;
        }

        [PunRPC] public void RPCServerSpawnConfirmation(int playerID, int selectedTeamID) //el server va a decidir cuando spawnean todos
        {
            _pointerConfirmationList[selectedTeamID][playerID] = true; //setear la confirmacion del player que llamo esta funcion

            if (_pointerConfirmationList[selectedTeamID].Any(x => !x)) //si posee alguno que es falso
                return;

            //si no posee ninguno falso, es que todos son verdaderos y confirmaron, por lo que:
            photonView.RPC("RPCSpawnAllTeam", RpcTarget.AllBuffered, selectedTeamID);
        }

        [PunRPC] public void RPCSpawnAllTeam(int selectedTeam) //se llama desde el server hacia los del team para que spawneen todos a la vez.
        {
            if (teamID != selectedTeam) return;

            StartCoroutine(SpawnDelayFeedback());
        }

        private IEnumerator SpawnDelayFeedback()
        {
            yield return new WaitForSeconds(1.5f);
            _panelOpened = false;
            Vector3 spawnPos = new Vector3(_spawnPointPos.x, 4f, _spawnPointPos.y) * mapScale;
            FindObjectOfType<GameCanvas>().SwitchMapPanel(false);
            FindObjectOfType<Server>().photonView.RPC("RPCInstantiateSpawnPoint", RpcTarget.MasterClient, teamID, spawnPos);
            FindObjectOfType<Server>().photonView.RPC("RPCRespawnPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, spawnPos);
            FindObjectOfType<LevelManager>().DestroyAllInitialSpawnPoints();
            Vector3 corePos = new Vector3(_corePointer.rectTransform.anchoredPosition.x, 4f, _corePointer.rectTransform.anchoredPosition.y) * mapScale;
            if(playerID == 0) //si es el primero en spawnear, entonces que spawnee tambien el core
                FindObjectOfType<Server>().photonView.RPC("RPCTeamSpawn", RpcTarget.MasterClient, teamID, corePos);
        }

        public void StartPanelTimer() => StartCoroutine(SecondsCoroutine());

        private void Update()
        {
            if (!_panelOpened) return;

            Vector3 pos = _allPointers[playerID].rectTransform.anchoredPosition;
            pos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            _coordText.text = "Coord: {" + -pos.x + "," + pos.y + "}";

            PointerMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        private bool _panelOpened;
        private IEnumerator SecondsCoroutine()
        {
            _panelOpened = true;
            int time = 0;
            while (_panelOpened)
            {
                time++;
                yield return new WaitForSeconds(1f);
                _timeText.text = time.ToString("0:00");
            }
        }
    }
}
