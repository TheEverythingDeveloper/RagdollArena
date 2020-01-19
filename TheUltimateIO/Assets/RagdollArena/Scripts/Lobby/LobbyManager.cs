using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyManager : MonoBehaviourPun
    {
        [SerializeField] private TMP_InputField _nicknameInputField;
        [SerializeField] private GameObject _nicknamePanel;
        public int sceneID;

        public void StartButton(int newSceneID)
        {
            sceneID = newSceneID;
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

        public void StartGame() //intentar conectarse a una sala existente o crear una
        {
            NetManager.Instance.Connect(sceneID);
        }

        public void BackToMenuButton()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

