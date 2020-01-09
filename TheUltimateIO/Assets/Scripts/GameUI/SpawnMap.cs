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
        private float _mapPointerPosScaler = 1.43f;
        [Tooltip("Literal es un multiplicador de cuanto va a ser el ancho y largo del mapa")] public float mapScale = 0.5f;

        public int playerTeamID; //numero de jugador que es el player en el equipo
        public int teamID; //numero de equipo para saber color
        private Vector3 _spawnPointPos;

        private Dictionary<int, bool[]> _pointerConfirmationList = new Dictionary<int, bool[]>();

        [SerializeField] private SpawnMapArea _spawnMapArea;

        public void UpdatePlayerIDs(int newPlayerTeamID, int newTeamID) //esto se llama en cada jugador, actualizando estos valores de la clase
        {
            playerTeamID = newPlayerTeamID;
            teamID = newTeamID;
        }

        public void SetTeamAmountOfPlayers(int teamsAmount, int playersAmount) //esto se tiene que reproducir solo en el servidor
        {
            _pointerConfirmationList.Clear();

            for (int i = 0; i < teamsAmount; i++)
                _pointerConfirmationList.Add(i, new bool[playersAmount]); //se van a agregar la cantidad de confirmaciones en false como players
        }

        [PunRPC] public void RPCUpdatePointer(int playerID, int selectedTeamID, Vector3 pos)
        {
            if (selectedTeamID != playerTeamID) return;

            _allPointers[playerID].gameObject.SetActive(true);
            _allPointers[playerID].rectTransform.localPosition = -pos * _mapPointerPosScaler;
        }

        public void PointerClick()
        {
            Vector3 pos = _spawnMapArea.GetComponent<RectTransform>().position - Input.mousePosition;
            Debug.Log("Relative Pos " + pos);
            _spawnPointPos = pos;
            photonView.RPC("RPCUpdatePointer", RpcTarget.All, playerTeamID, teamID, pos);
        }   

        public void SpawnButton()
        {
            photonView.RPC("RPCPlayerSpawnConfirmed", RpcTarget.All, playerTeamID, teamID);
            photonView.RPC("RPCServerSpawnConfirmation", RpcTarget.MasterClient, playerTeamID, teamID);
        }

        [PunRPC] public void RPCPlayerSpawnConfirmed(int playerID, int selectedTeamID) //esto se llama cuando se apreta el boton de spawn
        {
            if (selectedTeamID != teamID) return;

            _allPointers[playerID].color = Color.cyan;
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
            FindObjectOfType<LevelManager>().RespawnPlayer(new Vector3(_spawnPointPos.x, 5, _spawnPointPos.y) * mapScale);
        }

        private IEnumerator SpawnDelayFeedback()
        {
            yield return new WaitForSeconds(1.5f);
            _panelOpened = false;
            FindObjectOfType<GameCanvas>().SwitchMapPanel(false);
        }

        public void StartPanelTimer() => StartCoroutine(SecondsCoroutine());

        private void Update()
        {
            if (!_panelOpened) return;

            Vector3 pos = _spawnMapArea.GetComponent<RectTransform>().position - Input.mousePosition * _mapPointerPosScaler;
            if (_spawnPointPos != Vector3.zero)
                pos = _spawnPointPos;

            pos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            _coordText.text = "Coord: {" + -pos.x + "," + pos.y + "}";
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
