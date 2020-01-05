using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Server : MonoBehaviourPun
{
    private Dictionary<Player, CharacterModel> _allPlayers = new Dictionary<Player, CharacterModel>();
    private LevelManager _lvlMng;
    public int PackagesPerSecond { get; private set; }

    private void Awake()
    {
        _lvlMng = FindObjectOfType<LevelManager>();

        if (!photonView.IsMine) return;

        PackagesPerSecond = 30;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        _lvlMng.SwitchEnterToStartText(true);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int counter = 0;
            foreach (var player in _allPlayers)
            {
                counter++;
                Debug.Log("<color=blue>PLAYER "+counter+" = </color>" + player.Key.NickName);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(StartGameCoroutine(5));
    }

    IEnumerator StartGameCoroutine(int waitSeconds)
    {
        _lvlMng.SwitchEnterToStartText(false);
        for (int i = 0; i < waitSeconds; i++) //5..4..3..2..1.. GO.
        {
            photonView.RPC("RPCUpdateCounter", RpcTarget.All, waitSeconds - i); //es All y no allbuffered porque el que se conecte tarde tiene que empezar sin el conteo.
            yield return new WaitForSeconds(1f);
        }
        photonView.RPC("RPCUpdateCounter", RpcTarget.All, 0);
        yield return new WaitForSeconds(1f);
        photonView.RPC("RPCStartGame", RpcTarget.AllBuffered); //con allbuffered empezar el juego aun si llegaste tarde a la partida
    }

    [PunRPC] private void RPCUpdateCounter(int i)
    {
        _lvlMng.SwitchCounterPanel(true);
        Debug.Log("<color=green>" + i + "</color>");
        _lvlMng.CounterUpdate(i);
    }

    [PunRPC] private void RPCStartGame()
    {
        _lvlMng.SwitchCounterPanel(false);
        Debug.Log("<color=yellow> GO!!! </color>");
        int i = 0;
        foreach (var player in _allPlayers) //cambiar de team y spawnear correctamente a los jugadores de cada team en su posicion correspondiente.
        {
            i++;
            player.Value.StartGame(((i-1) % 2) + 1, player.Value.transform.position + (Vector3.up * 5));
        }
    }

    public void AddPlayer(Player newPhotonPlayer) => photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, newPhotonPlayer, true);
    public void RemovePlayer(Player toRemovePhotonPlayer)
    {
        photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, toRemovePhotonPlayer, false);
    }
    [PunRPC]
    private void RPCChangePlayer(Player photonPlayer, bool add)
    {
        if (!add) //si estamos removiendo un jugador
        {
            if (!_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que NO este en la lista, return
            Debug.Log("<color=red>Se fue de la partida un usuario!</color>");
            PhotonNetwork.Destroy(_allPlayers[photonPlayer].gameObject);
            _allPlayers.Remove(photonPlayer);
        }
        else
        {
            if (_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que ya este en la lista, return
            CharacterModel model = LevelManager.Instance.SpawnUser();
            Debug.Log("<color=green>Se unio a la partida un usuario! Se llama </color>"+photonPlayer.NickName);
            _allPlayers.Add(photonPlayer, model);
            _allPlayers[photonPlayer].photonView.RPC("RPCSetModelOwner", photonPlayer, true);
            _allPlayers[photonPlayer].photonView.RPC("RPCArtificialAwake", photonPlayer);
        }
    }
}
