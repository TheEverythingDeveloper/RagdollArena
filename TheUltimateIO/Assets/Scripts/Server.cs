using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviourPun
{
    Dictionary<Player, CharacterModel> _allPlayers = new Dictionary<Player, CharacterModel>();
    public static Server Instance;
    public int PackagesPerSecond { get; private set; }

    private void Awake()
    {
        if (!photonView.IsMine) return;

        PackagesPerSecond = 30;
        Instance = this;
        photonView.RPC("SetInstance", RpcTarget.AllBuffered, PackagesPerSecond);
    }
    [PunRPC]
    private void SetInstance(int packages) //setear instancia de singleton para cada usuario posible
    {
        Instance = this;
        PackagesPerSecond = packages;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        AddPlayer(photonView.Controller, LevelManager.Instance.SpawnUser());
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
    }

    public void AddPlayer(Player newPhotonPlayer, CharacterModel newModel) => photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, newPhotonPlayer, newModel);
    public void RemovePlayer(Player toRemovePhotonPlayer) => photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, toRemovePhotonPlayer, null);
    [PunRPC]
    private void RPCChangePlayer(Player photonPlayer, CharacterModel model)
    {
        if (model == null) //si estamos removiendo un jugador
        {
            if (!_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que NO este en la lista, return
            Debug.Log("<color=red>Se fue de la partida un usuario!</color>");
            _allPlayers.Remove(photonPlayer);
        }
        else
        {
            if (_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que ya este en la lista, return
            Debug.Log("<color=green>Se unio a la partida un usuario! Se llama </color>"+photonPlayer.NickName);
            _allPlayers.Add(photonPlayer, model);
        }
    }
}
