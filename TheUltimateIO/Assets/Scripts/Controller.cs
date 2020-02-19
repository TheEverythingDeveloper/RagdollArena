using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Character;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviourPun, IUpdatable
{
    Server _server;
    public bool controlsActive = true;

    private void Start()
    {
        if (FindObjectOfType<NetSpawner>()) Destroy(gameObject);
        if (!photonView.IsMine) return;
        FindObjectOfType<Chat>().SuscribeChat(ChatActive);

        StartCoroutine(DelayWaitForServer()); //esta corrutina se llama porque el controller se crea antes que el server. Esto es ya que el server esperaba a los demas jugadores.
    }

    void ChatActive(bool active)
    {
        controlsActive = active;
    }

    IEnumerator DelayWaitForServer()
    {
        yield return new WaitForSecondsRealtime(0.75f);

        _server = FindObjectOfType<Server>();
        _server.AddPlayer(photonView.Controller);
    }

    private void Update()
    {
        if (!photonView.IsMine || !controlsActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("<color=red>Desconectado de la partida actual</color>");
            _server.RemovePlayer(photonView.Controller);
            PhotonNetwork.Disconnect();
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("<color=red>Fuiste asesinado</color>");
            _server.photonView.RPC("RPCPlayerDeath", RpcTarget.MasterClient, photonView.Controller);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("<color=yellow>Intentando Respawnear</color>");
            FindObjectOfType<SpawnPoint>().UseSpawnPoint(photonView.Controller);
        }
        Move();
    }

    private void Move()
    {
        var horAxis = Input.GetAxis("Horizontal");
        var verAxis = Input.GetAxis("Vertical");

        if (horAxis != 0 || verAxis != 0)
        {
            FindObjectOfType<Server>().MovePlayer(photonView.Controller, horAxis, verAxis);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            FindObjectOfType<Server>().JumpPlayer(photonView.Controller);
    }

    public void ArtificialUpdate() { }

    public void ArtificialFixedUpdate() { }

    public void ArtificialLateUpdate() { }
}
