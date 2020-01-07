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
    Dictionary<int, string> _typesMsg;
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
        SendMsg("joined");
    }

    public void SendMsg(string msg)
    {
        photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msg, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    void RPCGlobalSendMsg(string msg, string name)
    {
        msgArea.text += "<color=green> |GLOBAL| </color>" + name + ": " + msg;
    }

    [PunRPC]
    void RPCTeamSendMsg(string msg, string name)
    {
        msgArea.text += "<color=red> |GLOBAL| </color>" + name + ": " + msg;
    }

    [PunRPC]
    void RPCPrivateSendMsg(string msg, string name)
    {
        msgArea.text += "<color=blue> |GLOBAL| </color>" + name + ": " + msg;
    }

}
