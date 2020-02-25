using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ExitPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    public void SwitchPanel(bool active)
    {
        _panel.SetActive(active);
    }

    public void OnExitButton()
    {
        FindObjectOfType<Server>().RemovePlayer(PhotonNetwork.LocalPlayer);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public void OnStayButton()
    {
        SwitchPanel(false);
    }
}
