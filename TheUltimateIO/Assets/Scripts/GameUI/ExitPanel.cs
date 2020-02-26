using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ExitPanel : MonoBehaviourPun
{
    [SerializeField] private GameObject _panel;

    public void SwitchPanel(bool active)
    {
        _panel.SetActive(active);
    }

    public void OnExitButton()
    {
        photonView.RPC("RPCExitPlayer", RpcTarget.MasterClient);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    [PunRPC] void RPCExitPlayer()
    {
        FindObjectOfType<Server>().RemovePlayer(PhotonNetwork.LocalPlayer);
    }

    public void OnStayButton()
    {
        SwitchPanel(false);
    }
}
