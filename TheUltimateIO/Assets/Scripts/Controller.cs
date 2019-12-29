using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Character;

public class Controller : MonoBehaviourPun
{
    private void Start()
    {
        if (!photonView.IsMine) return;

        StartCoroutine(DelayWaitForServer()); 
    }

    IEnumerator DelayWaitForServer()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        FindObjectOfType<Server>().AddPlayer(photonView.Controller);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("<color=red>Desconectado de la partida actual</color>");
            PhotonNetwork.Disconnect();
        }
    }
}
