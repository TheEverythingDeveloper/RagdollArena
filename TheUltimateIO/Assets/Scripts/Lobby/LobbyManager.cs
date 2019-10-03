using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace Lobby
{
    public class LobbyManager : MonoBehaviourPun
    {
        private NetManager _netMng;
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private GameObject _nicknamePanel;

        private void Awake()
        {
            _netMng = FindObjectOfType<NetManager>();
        }

        public void StartButton()
        {
            _nicknamePanel.SetActive(true);
        }

        public void OnEndValue()
        {
            if (_nicknameInputField.text == "" || _nicknameInputField.text == "Insert a nickname")
            {
                _nicknameInputField.text = "Insert a nickname";
                return;
            }
            PhotonNetwork.NickName = _nicknameInputField.text;
            _nicknameInputField.text = "Starting...";
            _nicknameInputField.enabled = false;
            StartGame();
        }

        public void StartGame()
        {
            _netMng.Connect();
        }
    }
}

