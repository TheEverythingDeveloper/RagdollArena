using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviourPun
{
    Dictionary<Player, CharacterModel> _allPlayers = new Dictionary<Player, CharacterModel>();
    public static Server Instance { get; private set; }
    public int PackagePerSecond { get; private set; }

    private void Awake()
    {
        Instance = this;
        AddPlayer(photonView.Controller, FindObjectOfType<CharacterModel>());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var player in _allPlayers)
                Debug.Log("<color=red>PLAYER = </color>" + player.Key.NickName);
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
            _allPlayers.Remove(photonPlayer);
        }
        else
        {
            if (_allPlayers.ContainsKey(photonPlayer)) return; //en caso de que ya este en la lista, return
            _allPlayers.Add(photonPlayer, model);
        }
    }
}
