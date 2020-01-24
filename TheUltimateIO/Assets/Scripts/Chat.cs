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
    public EmojisChat emojisPanel;
    private bool _emojisPanelActive;
    public InputField msgInput;
    Dictionary<int, string> _typesMsg = new Dictionary<int, string>();
    //public Dropdown typeMsg;
    public GameObject content;
    public TextMeshProUGUI textMsg;

    public List<Color> colorsTeam;
    Action _chatActive;

    private CharacterModel _characterModel;
    [HideInInspector] public Controller controller;

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
        if (Input.GetKeyDown(KeyCode.Escape) && _emojisPanelActive) ActivePanelEmojis(false);
    }

    public void InitializedChat(CharacterModel model)
    {
        _characterModel = model;
        emojisPanel.InitializedEmojis(this);
    }

    public void ActivePanelEmojis(bool active)
    {
        emojisPanel.gameObject.SetActive(active);
        _emojisPanelActive = active;
    }

    public void OnConnected()
    {
        photonView.RPC("RPCGlobalSendMsg", RpcTarget.All, "joined", PhotonNetwork.LocalPlayer.NickName);
    }

    public void OnInputMsg(bool enter)
    {
        if (enter) _chatActive = ChatControllerActive;
        else _chatActive = ChatControllerOff;

        controller.controlsActive = !enter;

        FindObjectOfType<Server>().photonView.RPC("RPCActivateChat", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, enter);
    }

    public void SendMsg()
    {
        if(msgInput.text != "")
        {
            photonView.RPC("RPCGlobalSendMsg", RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName);
            /*
            if (typeMsg.value != 1)
                photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName);
            else
                photonView.RPC(_typesMsg[typeMsg.value], RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName, _characterModel.team);*/
        }

        OnInputMsg(false);
        //msgInput.MoveTextEnd(false);
        msgInput.text = "";
    }

    [PunRPC]
    void RPCGlobalSendMsg(string msg, string name)
    {
        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=green> |GLOBAL| </color>" + name + ": " + msg;
        newText.transform.SetParent(content.transform);
        newText.rectTransform.localScale = Vector3.one;
    }

    [PunRPC]
    void RPCTeamSendMsg(string msg, string name, int team)
    {
        if (team != _characterModel.team) return;

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
        newText.transform.SetParent(content.transform);
        newText.rectTransform.localScale = Vector3.one;
    }
    #endregion
}
