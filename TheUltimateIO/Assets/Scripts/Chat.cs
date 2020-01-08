using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    public Text msgInput;
    public Text msgArea;
    Dictionary<int, string> _typesMsg = new Dictionary<int, string>();
    public Dropdown typeMsg;

    void Start()
    {
        //        if (!photonView.IsMine) return;
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
        msgArea.text += "<color=green> |GLOBAL| </color>" + name + ": " + msg;
    }

    [PunRPC]
    void RPCTeamSendMsg(string msg, string name)
    {
        msgArea.text += "<color=red> |TEAM| </color>" + name + ": " + msg;
    }

    [PunRPC]
    void RPCPrivateSendMsg(string msg, string name)
    {
        msgArea.text += "<color=blue> |PRIVATE| </color>" + name + ": " + msg;
    }

}
