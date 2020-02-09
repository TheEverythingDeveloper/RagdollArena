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
    public GameObject content;
    public TextMeshProUGUI textMsg;

    public List<Color> colorsTeam;
    private Action _chatActive;

    private CharacterModel _characterModel;
    [HideInInspector] public Controller controller;

    private List<Action<bool>> _metodsSuscribes = new List<Action<bool>>();

    public delegate void ConsoleCommand(string txt);
    private Dictionary<string, ConsoleCommand> _myCommands = new Dictionary<string, ConsoleCommand>();

    private void Awake()
    {
        _chatActive = ChatControllerOff;
        SuscribeChat(ChangeControls);
    }

    void Start()
    {
        AddCommands("/private", PrivateSendMsg);
        AddCommands("/team", TeamSendMsg);

        OnConnected();
    }

    private void Update()
    {
        _chatActive();
        if (Input.GetKeyDown(KeyCode.Escape) && _emojisPanelActive) ActivePanelEmojis(false);
    }

    public void SuscribeChat(Action<bool> metod)
    {
        _metodsSuscribes.Add(metod);
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
        foreach (var item in _metodsSuscribes)
        {
            item(!enter);
        }

        FindObjectOfType<Server>().photonView.RPC("RPCActivateChat", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, enter);
    }

    void ChangeControls(bool active)
    {
        if (active) _chatActive = ChatControllerActive;
        else _chatActive = ChatControllerOff;
    }

    public void SendMsg()
    {
        if(CheckCommand()) return;

        if (msgInput.text != "")
        {
            photonView.RPC("RPCGlobalSendMsg", RpcTarget.All, msgInput.text, PhotonNetwork.LocalPlayer.NickName);
        }

        OnInputMsg(false);
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

    public void TeamSendMsg(string txt)
    {
        var command = txt.Split(' ');
        var msg = "";

        for (int i = 1; i < command.Length; i++)
        {
            msg += " " + command[i];
        }

        photonView.RPC("RPCTeamSendMsg", RpcTarget.All, msg, PhotonNetwork.LocalPlayer.NickName, _characterModel.team);
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

    public void PrivateSendMsg(string txt)
    {
        var command = txt.Split(' ');
        var nameSend = command[1];

        var msg = "";

        for (int i = 2; i < command.Length; i++)
        {
            msg += " " + command[i];
        }

        photonView.RPC("RPCPrivateSendMsg", RpcTarget.All, msg, PhotonNetwork.LocalPlayer.NickName, nameSend);
    }

    [PunRPC]
    void RPCPrivateSendMsg(string msg, string namePlayer, string nameSendMsg)
    {
        if (_characterModel.nickname != nameSendMsg && PhotonNetwork.LocalPlayer.NickName != namePlayer) return;

        var newText = Instantiate(textMsg);
        newText.gameObject.SetActive(true);
        newText.text = "<color=blue> " + namePlayer + " ---> " + nameSendMsg + "</color> : " + msg;
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

    public void AddCommands(string cheat, ConsoleCommand com)
    {
        _myCommands.Add(cheat, com);
    }

    public bool CheckCommand()
    {
        if (msgInput.text.StartsWith("/"))
        {
            var command = msgInput.text.Split(' ');
            if (_myCommands.ContainsKey(command[0]))
            {
                _myCommands[command[0]](msgInput.text);
            }
            else
            {
                var newText = Instantiate(textMsg);
                newText.gameObject.SetActive(true);
                newText.text = "<color=red> The command does not exist";
                newText.transform.parent = content.transform;
                newText.rectTransform.localScale = Vector3.one;
            }

            OnInputMsg(false);
            msgInput.text = "";

            return true;
        }

        return false;
    }
}
