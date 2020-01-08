using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Chat : MonoBehaviourPun
{
    public GameObject chatArea;
    public Text msgInput;
    Dictionary<int, string> _typesMsg = new Dictionary<int, string>();
    public Dropdown typeMsg;
    public GameObject content;
    public TextMeshProUGUI textMsg;

    void Start()
    {
        _typesMsg.Add(0, "RPCGlobalSendMsg");
        _typesMsg.Add(1, "RPCTeamSendMsg");
        _typesMsg.Add(2, "RPCPrivateSendMsg");
        OnConnected();
    }

    public void OnConnected()
    {
        photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, "joined", PhotonNetwork.LocalPlayer.NickName);
    }

    public void SendMsg()
    {
        photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName);
        msgInput.text = "";
    }

    [PunRPC]
    void RPCGlobalSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=green> |GLOBAL| </color>" + name + ": " + msg;
        newText.transform.parent = content.transform;
    }

    [PunRPC]
    void RPCTeamSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=red> |TEAM| </color>" + name + ": " + msg;
        newText.transform.parent = content.transform;
    }

    [PunRPC]
    void RPCPrivateSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=blue> |PRIVATE| </color>" + name + ": " + msg;
        newText.transform.parent = content.transform;
    }

    public void OpenOrCloseChat(bool open)
    {
        chatArea.SetActive(open);
    }
}
