using Character;
using Photon.Pun;
using Photon.Realtime;
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
    }

    public void AddPlayer(Player newPhotonPlayer) => photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, newPhotonPlayer, true);
    public void RemovePlayer(Player toRemovePhotonPlayer)
    {
        //if (toRemovePhotonPlayer == null) //lo cual significa que no se sabe cual se removio, pero se sabe que se removio alguien..
        //{
        //    photonView.RPC("RPCAnalyzeLeftPlayer", RpcTarget.MasterClient);
        //    return;
        //}
        photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, toRemovePhotonPlayer, false);
    }
    [PunRPC]
    private void RPCChangePlayer(Player photonPlayer, bool add)
    {
        if (!add) //si estamos removiendo un jugador
        {
            if (!_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que NO este en la lista, return
            Debug.Log("<color=red>Se fue de la partida un usuario!</color>");
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
