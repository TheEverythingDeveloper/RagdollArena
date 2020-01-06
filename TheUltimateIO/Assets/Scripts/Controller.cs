using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Character;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviourPun
{
    Server server;
    private void Start()
    {
        if (!photonView.IsMine) return;

        StartCoroutine(DelayWaitForServer()); //esta corrutina se llama porque el controller se crea antes que el server. Esto es ya que el server esperaba a los demas jugadores.
    }

    IEnumerator DelayWaitForServer()
    {
        yield return new WaitForSecondsRealtime(0.75f);

        server = FindObjectOfType<Server>();
        server.AddPlayer(photonView.Controller);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("<color=red>Desconectado de la partida actual</color>");
            FindObjectOfType<Server>().RemovePlayer(photonView.Controller);
            PhotonNetwork.Disconnect();
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }

        //if (Input.GetKeyDown(KeyCode.Space))
            //_myModel.TryJump();
        /*if (Input.GetKeyDown(KeyCode.E))
            _myModel.TryGrenade();
        if (Input.GetKeyUp(KeyCode.E))
            _myModel.ThrowGrenade();
        if (Input.GetKeyDown(KeyCode.R))
            _myModel.TryGrenadeDrunk();*/
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var horAxis = Input.GetAxis("Horizontal");
        var verAxis = Input.GetAxis("Vertical");

        if (horAxis != 0 || verAxis != 0)
            server.MovePlayer(photonView.Controller, horAxis, verAxis);       
    }
}
