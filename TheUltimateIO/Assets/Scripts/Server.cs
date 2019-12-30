using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviourPun
{
    Dictionary<Player, CharacterModel> _allPlayers = new Dictionary<Player, CharacterModel>();
    public int PackagesPerSecond { get; private set; }

    private void Awake()
    {
        if (!photonView.IsMine) return;

        PackagesPerSecond = 30;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        AddPlayer(photonView.Controller);
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
        for (int i = 0; i < waitSeconds; i++) //5..4..3..2..1.. GO.
        {
            photonView.RPC("RPCUpdateCounter", RpcTarget.All, i); //es All y no allbuffered porque el que se conecte tarde tiene que empezar sin el conteo.
            yield return new WaitForSeconds(1f);
        }
        photonView.RPC("RPCStartGame", RpcTarget.AllBuffered); //con allbuffered empezar el juego aun si llegaste tarde a la partida
    }

    [PunRPC] private void RPCUpdateCounter(int i)
    {
        Debug.Log("<color=green>" + i + "</color>");
    }

    [PunRPC] private void RPCStartGame()
    {
        Debug.Log("<color=yellow> GO!!! </color>");
        //poner los equipos
        //
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
        }
    }
}
