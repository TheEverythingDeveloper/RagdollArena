﻿using Character;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using GameUI;
using Friends;
using Audio;
using Random = UnityEngine.Random;

public class Server : MonoBehaviourPun
{
    public Dictionary<Player, CharacterModel> allPlayers = new Dictionary<Player, CharacterModel>();
    private LevelManager _lvlMng;
    public int PackagesPerSecond { get; private set; }

    public bool controlsActive = true;
    Chat _chat;
    public int minPlayersToPlay = 2;
    public bool readyToPlay = true;
    public bool startGame;
    public int timeConstructInSeconds;

    private void Awake()
    {
        if (FindObjectOfType<NetSpawner>()) Destroy(gameObject);

        _lvlMng = FindObjectOfType<LevelManager>();

        if (!photonView.IsMine) return;

        _chat = FindObjectOfType<Chat>();
        CheckReadyToPlay();
        PackagesPerSecond = 30;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        FindObjectOfType<Chat>().SuscribeChat(ChatActive);

        _lvlMng.gameCanvas.SwitchEnterToStartText(true);
    }

    private void Update()
    {
        if (!photonView.IsMine || !controlsActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int counter = 0;
            foreach (var player in allPlayers)
            {
                counter++;
                Debug.Log("<color=blue>PLAYER " + counter + " = </color>" + player.Key.NickName);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && readyToPlay)
            StartCoroutine(StartGameCoroutine(5));

    }

    void CheckReadyToPlay()
    {
        if (startGame) return;

        if(allPlayers.Count >= minPlayersToPlay && !readyToPlay)
        {
            FindObjectOfType<GameCanvas>().SwitchChangeStartText("Press Enter to start game");
            readyToPlay = true;
        }
        else if(allPlayers.Count < minPlayersToPlay && readyToPlay)
        {
            FindObjectOfType<GameCanvas>().SwitchChangeStartText("Minimum 2 players to play");
            readyToPlay = false;
        }
    }
    void ChatActive(bool active)
    {
        Debug.LogError("chat" + active);
        controlsActive = active;
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
        foreach (var player in allPlayers)
            player.Value.photonView.RPC("RPCStartGame", player.Key);

        StartCoroutine(TimerConstructMode());
        SpawnBricks(30);
        StartCoroutine(SpawnBricksCoroutine());
    }
    IEnumerator SpawnBricksCoroutine()
    {
        while(true)
        {
            SpawnBricks(_lvlMng.amountOfBricksPerSecond);
            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnBricks(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float rnd = Random.value;
            PhotonNetwork.Instantiate(rnd > 0.9f ? "BricksBig" : rnd > 0.65f ? "BricksMedium" : "BrickSmall", 
                _lvlMng.PrecisionPositionRandom() + Vector3.up * 50, Quaternion.identity);
        }
    }
    IEnumerator TimerConstructMode()
    {
        photonView.RPC("RPCPosterConstruction", RpcTarget.All);
        yield return new WaitForSeconds(timeConstructInSeconds);
        photonView.RPC("RPCPosterWar", RpcTarget.All);

    }
    [PunRPC] void RPCPosterConstruction() { MsgServerUI.Instance.NewMsg("In " + timeConstructInSeconds + " seconds the war will begin, build to protect the nexus.", 5); }

    [PunRPC] void RPCPosterWar() { MsgServerUI.Instance.NewMsg("Construction time is over, go to the war!!", 5); FindObjectOfType<ServerAudioManager>().PlaySound(0); }

    [PunRPC] private void RPCUpdateCounter(int i)
    {
        _lvlMng.gameCanvas.SwitchCounterPanel(true);
        Debug.Log("<color=green>" + i + "</color>");
        _lvlMng.gameCanvas.CounterUpdate(i);
        if(i == 0) startGame = true;
    }
    [PunRPC] public void RPCChangePlayerTeam(Player photonPlayer, int teamID) => allPlayers[photonPlayer].photonView.RPC("RPCChangePlayerTeam", photonPlayer, teamID);
    public void AddPlayer(Player newPhotonPlayer) { photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, newPhotonPlayer, true); photonView.RPC("RPCAddPlayerFriend", RpcTarget.AllBuffered, newPhotonPlayer.NickName); }
    [PunRPC] private void RPCAddPlayerFriend(string namePlayer) { FindObjectOfType<FriendSystem>().AddButtonFriend(namePlayer); }
    public void RemovePlayer(Player toRemovePhotonPlayer)
    {
        if (!PhotonNetwork.IsConnected) return;
        photonView.RPC("RPCChangePlayer", RpcTarget.MasterClient, toRemovePhotonPlayer, false);
    }
    [PunRPC] private void RPCChangePlayer(Player photonPlayer, bool add)
    {
        if (!add) //si estamos removiendo un jugador
        {
            if (!allPlayers.ContainsKey(photonPlayer)) return; //en caso de que NO este en la lista, return
            Debug.Log("<color=red>Se fue de la partida un usuario!</color>");
            _chat.SendMsgServer(photonPlayer.NickName + " disconnected");
            PhotonNetwork.Destroy(allPlayers[photonPlayer].gameObject);
            allPlayers.Remove(photonPlayer);
            FindObjectOfType<TeamManager>().RemovePlayer(photonPlayer);
            CheckReadyToPlay();
        }
        else
        {
            if (allPlayers.ContainsKey(photonPlayer)) return; //en caso de que ya este en la lista, return
            CharacterModel model = LevelManager.Instance.SpawnUser();
            Debug.Log("<color=green>Se unio a la partida un usuario! Se llama </color>" + photonPlayer.NickName);
            allPlayers.Add(photonPlayer, model);
            allPlayers[photonPlayer].photonView.RPC("RPCSetModelOwner", photonPlayer, true, photonPlayer);
            allPlayers[photonPlayer].photonView.RPC("RPCArtificialAwake", RpcTarget.AllBuffered);
            FindObjectOfType<TeamManager>().AddPlayer(photonPlayer);
            CheckReadyToPlay();
        }
    }
    public void ChangeControls(Player photonPlayer, Action<float, float> move, Rigidbody newRb)
    {
        allPlayers[photonPlayer].ChangeControls(move, newRb);
    }
    [PunRPC] public void RPCPlayerDeath(Player photonPlayer) //Decirle al modelo que se apague y que ponga los paneles de respawn en el HUD
    {
        photonView.RPC("RPCChangeRespawnFeedback", photonPlayer, true);
        allPlayers[photonPlayer].photonView.RPC("RPCChangeRespawnMode", RpcTarget.AllBuffered, true);
    }
    public void MovePlayer(Player photonPlayer, float horAxis, float verAxis)
    { photonView.RPC("RPCMovePlayer", RpcTarget.MasterClient, photonPlayer, horAxis, verAxis); }
    [PunRPC] private void RPCMovePlayer(Player photonPlayer, float horAxis, float verAxis)
    {
        if (!allPlayers.ContainsKey(photonPlayer)) return;

        allPlayers[photonPlayer].MovePlayer(horAxis, verAxis);
    }

    public void JumpPlayer(Player photonPlayer) { photonView.RPC("RPCJumpPlayer", RpcTarget.MasterClient, photonPlayer); }
    [PunRPC] private void RPCJumpPlayer(Player photonPlayer)
    {
        if (!allPlayers.ContainsKey(photonPlayer)) return;

        allPlayers[photonPlayer].TryJump();
    }
    [PunRPC] public void RPCRespawnPlayer(Player photonPlayer, Vector3 position)
    {
        Debug.Log("<color=green>Respawneado en  " + position + "</color>");

        photonView.RPC("RPCChangeRespawnFeedback", photonPlayer, false);
        allPlayers[photonPlayer].photonView.RPC("RPCRespawn", RpcTarget.AllBuffered, position);
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
    [PunRPC] private void RPCChangeRespawnFeedback(bool dead) { OnRespawnFeedback(dead); }
    public bool DamageActive() { return startGame; }
    [PunRPC] public void RPCEndGame(int teamID, Player[] winnerPlayers)
    {
        winnerPlayers.Select(x =>
        {
            Debug.Log("<color=yellow>WINNER = " + x.NickName + "</color>");
            return x;
        }).ToList();

        PhotonNetwork.Instantiate("EndPanel", Vector3.zero, Quaternion.identity);
        photonView.RPC("RPCUpdateEndPanel", RpcTarget.All, teamID, winnerPlayers);
    }

    [PunRPC] public void RPCUpdateEndPanel(int winnerTeam, Player[] winners)
    { FindObjectOfType<EndPanel>().UpdateWinnersData(winnerTeam, winners); }

    public void StartRematch(List<Player> allRematchedPlayers)
    {
        StopCoroutine(SpawnBricksCoroutine());
        var allSpawnPoints = FindObjectsOfType<SpawnPoint>();
        var allBricks = FindObjectsOfType<Brick>();
        foreach (var x in allBricks)
            PhotonNetwork.Destroy(x.gameObject);
        foreach (var x in allSpawnPoints)
            PhotonNetwork.Destroy(x.gameObject);
        photonView.RPC("RPCStartRematch", RpcTarget.All);
        foreach (var x in allPlayers)
            RPCRespawnPlayer(x.Key, _lvlMng.PositionRandom());

        PhotonNetwork.Destroy(FindObjectOfType<EndPanel>().gameObject);
        Core lastCore = FindObjectOfType<Core>();
        if (lastCore != null)
            lastCore.OnCoreDestroy(lastCore.teamID);

        //TODO: Destruir todo tipo de construccion y particulas que se hayan generado antes, destruir todo tipo de objeto de sonido

        _lvlMng.gameCanvas.SwitchEnterToStartText(true);

        //TODO: Hacer algo con los que no se rematchearon. Sacarlos de aca (que son los que quedan en la variable temp)
        /*if(_allPlayers.Count > 1)
        {
            Dictionary<Player, CharacterModel> temp = new Dictionary<Player, CharacterModel>(_allPlayers);
            foreach (var x in allRematchedPlayers)
                temp.Remove(x);
            foreach (var x in temp)
                _allPlayers.Remove(x.Key);
        }*/

        FindObjectOfType<TeamManager>().RematchReorganization(allPlayers.Select(x => x.Key).ToList());
        FindObjectOfType<SpawnMap>().photonView.RPC("RPCResetAllPointers", RpcTarget.All);

    }

    [PunRPC] private void RPCStartRematch()
    {
        startGame = true;
        Instantiate(Resources.Load("InitialStateOfGame"));
        _lvlMng.SetInitialSpawnPoints();
    }

    [PunRPC] public void RPCActivateEmoji(Player emojiPlayer, int emojiID)
    {
        allPlayers[emojiPlayer].photonView.RPC("RPCActivateEmoji", RpcTarget.All, emojiID);
    }

    [PunRPC]
    public void RPCActivateChat(Player player, bool chatActive)
    {
        allPlayers[player].photonView.RPC("RPCActivateChat", RpcTarget.All, chatActive);
    }
}
