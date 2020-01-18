using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using GameUI;

public class Server : MonoBehaviourPun
{
    private Dictionary<Player, CharacterModel> _allPlayers = new Dictionary<Player, CharacterModel>();
    private LevelManager _lvlMng;
    public int PackagesPerSecond { get; private set; }

    public bool controlsActive = true;
    Chat _chat;
    public bool startGame;
    public int timeConstructInSeconds;
    private void Awake()
    {
        _lvlMng = FindObjectOfType<LevelManager>();

        if (!photonView.IsMine) return;

        PackagesPerSecond = 30;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        _chat = FindObjectOfType<Chat>();
        _chat.server = this;

        _lvlMng.gameCanvas.SwitchEnterToStartText(true);
    }

    private void Update()
    {
        if (!photonView.IsMine || !controlsActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int counter = 0;
            foreach (var player in _allPlayers)
            {
                counter++;
                Debug.Log("<color=blue>PLAYER " + counter + " = </color>" + player.Key.NickName);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(StartGameCoroutine(5));
    }
    IEnumerator StartGameCoroutine(int waitSeconds)
    {
        _lvlMng.gameCanvas.SwitchEnterToStartText(false);
        for (int i = 0; i < waitSeconds; i++) //5..4..3..2..1.. GO.
        {
            photonView.RPC("RPCUpdateCounter", RpcTarget.All, waitSeconds - i);
            yield return new WaitForSeconds(1f);
        }
        photonView.RPC("RPCUpdateCounter", RpcTarget.All, 0);
        yield return new WaitForSeconds(1f);
        foreach (var player in _allPlayers)
            player.Value.photonView.RPC("RPCStartGame", player.Key);

        StartCoroutine(TimerConstructMode());
    }
    IEnumerator TimerConstructMode()
    {
        _chat.SendMsgServer("In " + timeConstructInSeconds + " seconds the war will begin, build to protect the nexus.");
        yield return new WaitForSeconds(timeConstructInSeconds);
        _chat.SendMsgServer("Construction time is over, go to the war!!");
        startGame = true;
        //TODO: Feedback de que comienza la guerra.
    }
    [PunRPC] private void RPCUpdateCounter(int i)
    {
        _lvlMng.gameCanvas.SwitchCounterPanel(true);
        Debug.Log("<color=green>" + i + "</color>");
        _lvlMng.gameCanvas.CounterUpdate(i);
    }
    [PunRPC] public void RPCChangePlayerTeam(Player photonPlayer, int teamID) => _allPlayers[photonPlayer].photonView.RPC("RPCChangePlayerTeam", photonPlayer, teamID);
    public void AddPlayer(Player newPhotonPlayer) => photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, newPhotonPlayer, true);
    public void RemovePlayer(Player toRemovePhotonPlayer)
    {
        if (!PhotonNetwork.IsConnected) return;
        photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, toRemovePhotonPlayer, false);
    }
    [PunRPC] private void RPCChangePlayer(Player photonPlayer, bool add)
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
            Debug.Log("<color=green>Se unio a la partida un usuario! Se llama </color>" + photonPlayer.NickName);
            _allPlayers.Add(photonPlayer, model);
            _allPlayers[photonPlayer].photonView.RPC("RPCSetModelOwner", photonPlayer, true);
            _allPlayers[photonPlayer].photonView.RPC("RPCArtificialAwake", RpcTarget.AllBuffered);
            FindObjectOfType<TeamManager>().AddPlayer(photonPlayer);
        }
    }
    [PunRPC] public void RPCPlayerDeath(Player photonPlayer) //Decirle al modelo que se apague y que ponga los paneles de respawn en el HUD
    {
        photonView.RPC("RPCChangeRespawnFeedback", photonPlayer, true);
        _allPlayers[photonPlayer].photonView.RPC("RPCChangeRespawnMode", RpcTarget.AllBuffered, true);
    }
    public void MovePlayer(Player photonPlayer, float horAxis, float verAxis)
    {
        photonView.RPC("RPCMovePlayer", RpcTarget.MasterClient, photonPlayer, horAxis, verAxis);
    }
    [PunRPC] private void RPCMovePlayer(Player photonPlayer, float horAxis, float verAxis)
    {
        if (!_allPlayers.ContainsKey(photonPlayer)) return;

        _allPlayers[photonPlayer].MovePlayer(horAxis, verAxis);
    }

    public void JumpPlayer(Player photonPlayer)
    {
        photonView.RPC("RPCJumpPlayer", RpcTarget.MasterClient, photonPlayer);
    }
    [PunRPC] private void RPCJumpPlayer(Player photonPlayer)
    {
        if (!_allPlayers.ContainsKey(photonPlayer)) return;

        _allPlayers[photonPlayer].TryJump();
    }
    [PunRPC] public void RPCRespawnPlayer(Player photonPlayer, Vector3 position)
    {
        Debug.Log("<color=green>Respawneado</color>");

        photonView.RPC("RPCChangeRespawnFeedback", photonPlayer, false);
        _allPlayers[photonPlayer].photonView.RPC("RPCRespawn", RpcTarget.AllBuffered, position);
    }
    [PunRPC] public void RPCInstantiateSpawnPoint(int teamID, Vector3 pos)
    {
        var go = PhotonNetwork.Instantiate("SpawnPoint", pos, Quaternion.identity);
        go.GetComponentInChildren<SpawnPoint>().photonView.RPC("RPCSetTeam", RpcTarget.AllBuffered, teamID);
    }

    [PunRPC] public void RPCTeamSpawn(int teamID, Vector3 corePos)
    {
        var go = PhotonNetwork.Instantiate("Core", corePos + Vector3.up * 2, Quaternion.identity);
        go.GetComponentInChildren<Core>().photonView.RPC("RPCSetTeam", RpcTarget.AllBuffered, teamID);
    }

    public event Action<bool> OnRespawnFeedback = delegate { };
    [PunRPC] private void RPCChangeRespawnFeedback(bool dead)
    {
        OnRespawnFeedback(dead);
    }

    public bool DamageActive()
    {
        return startGame;
    }
}
