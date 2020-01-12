using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Character;
using System;

public class Chat : MonoBehaviourPun
{
    public GameObject chatArea;
    public InputField msgInput;
    Dictionary<int, string> _typesMsg = new Dictionary<int, string>();
    public Dropdown typeMsg;
    public GameObject content;
    public TextMeshProUGUI textMsg;

    public List<Color> colorsTeam;
    Action _chatActive;

    [HideInInspector] public CharacterModel characterModel;
    [HideInInspector] public Controller controller;
    [HideInInspector] public Server server;

    private void Awake()
    {
        _chatActive = ChatControllerOff;
    }

    void Start()
    {
        _typesMsg.Add(0, "RPCGlobalSendMsg");
        _typesMsg.Add(1, "RPCTeamSendMsg");
        _typesMsg.Add(2, "RPCPrivateSendMsg");
        OnConnected();
    }

    private void Update()
    {
        _chatActive();
    }

    public void OnConnected()
    {
        photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, "joined", PhotonNetwork.LocalPlayer.NickName);
    }

    public void OnInputMsg(bool enter)
    {
        if (enter) _chatActive = ChatControllerActive;
        else _chatActive = ChatControllerOff;

        controller.controlsActive = !enter;
        server.controlsActive = !enter;
    }

    public void SendMsg()
    {
        if(msgInput.text != default)
        {
            if (typeMsg.value != 1)
                photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName);
            else
                photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName, characterModel.team);
        }

        OnInputMsg(false);
        msgInput.MoveTextEnd(false);
        msgInput.text = "";
    }



    [PunRPC]
    void RPCGlobalSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=green> |GLOBAL| </color>" + name + ": " + msg;
        newText.transform.parent = content.transform;
        newText.rectTransform.localScale = Vector3.one;
    }

    [PunRPC]
    void RPCTeamSendMsg(string msg, string name, int team)
    {
        if (team != characterModel.team) return;

        string colorText = ColorUtility.ToHtmlStringRGB(colorsTeam[team]);

        var newText = Instantiate(textMsg);

        newText.gameObject.SetActive(true);
        newText.text = "<color=#"+ colorText + "> |TEAM| " + name + ": </color>" + msg;
        newText.transform.parent = content.transform;
        newText.rectTransform.localScale = Vector3.one;
    }

    [PunRPC]
    void RPCPrivateSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=blue> |PRIVATE| </color>" + name + ": " + msg;
        newText.transform.parent = content.transform;
        newText.rectTransform.localScale = Vector3.one;
    }

    public void OpenOrCloseChat(bool open)
    {
        chatArea.SetActive(open);
    }

    void ChatControllerActive()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SendMsg();
    }

    void ChatControllerOff()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            msgInput.Select();
        }
    }

    #region serverChat
    public void SendMsgServer(string msg)
    {
        photonView.RPC("RPCSendMsgServer", RpcTarget.All, msg);
    }

    [PunRPC]
    void RPCSendMsgServer(string msg)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=black> |SERVER| </color> : " + msg;
        newText.transform.parent = content.transform;
        newText.rectTransform.localScale = Vector3.one;
    }
    #endregion
}
