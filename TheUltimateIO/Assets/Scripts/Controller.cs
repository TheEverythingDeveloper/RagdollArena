using Photon.Pun;
using UnityEngine;

public class Controller : MonoBehaviourPun
{
    private void Start()
    {
        if (!photonView.IsMine) return;

        Server.Instance = FindObjectOfType<Server>();

        if (Server.Instance == null)
            Debug.LogError("SERVER INSTANCE");

        Server.Instance.AddPlayer(photonView.Controller, LevelManager.Instance.SpawnUser());
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
