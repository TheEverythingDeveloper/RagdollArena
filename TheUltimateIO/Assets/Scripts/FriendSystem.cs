using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using GameUI;

namespace Friends
{
    public class Friends
    {
        public List<string> namesFriends = new List<string>();
    }
    public class FriendSystem : MonoBehaviourPun
    {
        public Friends friends;
        public Chat chat;
        public GameObject panelPlayers;
        public GameObject contentButtons;
        public FriendButton prefabButtons;
        public TextMeshProUGUI textFriends;
        public List<Action> panelsRemoveActive = new List<Action>();
        List<FriendButton> _buttons = new List<FriendButton>();
        List<string> namesPlayers = new List<string>();
        string _jsonSavePath;
        bool _panelActive;

        [HideInInspector] public List<string> namesFriendsOn = new List<string>();
        private void Start()
        {
            friends = new Friends();
            _jsonSavePath = Application.persistentDataPath + "/friends.json";
            LoadDataFriends();
            CounterFriendsOn();
        }

        void CounterFriendsOn()
        {
            textFriends.text = "FRIENDS ON: " + NumberFriendsOn();
        }

        int NumberFriendsOn()
        {
            List<string> friendsConnected = new List<string>();

            foreach (var players in namesPlayers)
            {
                foreach (var friends in friends.namesFriends)
                {
                    if (friends == players)
                    {
                        friendsConnected.Add(friends);
                        break;
                    }
                }
            }

            namesFriendsOn = friendsConnected;
            return namesFriendsOn.Count;
        }
        public void OpenOrClosePanel()
        {
            _panelActive = !_panelActive;

            if (_panelActive)
                foreach (var panel in panelsRemoveActive)
                    panel();

            panelPlayers.SetActive(_panelActive);
        }

        public void AddButtonFriend(string name)
        {
            if (PhotonNetwork.NickName == name) return;

            var newButton = Instantiate(prefabButtons);
            newButton.gameObject.SetActive(true);
            newButton.transform.parent = contentButtons.transform;
            newButton.namePlayer = name;
            newButton.StartButton(this, friends);
            _buttons.Add(newButton);
            namesPlayers.Add(name);
            CounterFriendsOn();
        }

        public void RemoveFriend(string name)
        {
            friends.namesFriends.Remove(name);
            SaveDataFriends();
        }

        void SaveDataFriends()
        {
            string jsonData = JsonUtility.ToJson(friends, true);
            File.WriteAllText(_jsonSavePath, jsonData);
        }

        void LoadDataFriends()
        {
            if (System.IO.File.Exists(_jsonSavePath)) friends = JsonUtility.FromJson<Friends>(File.ReadAllText(_jsonSavePath));
            else SaveDataFriends();
        }

        public void AddFriend(string name)
        {
            friends.namesFriends.Add(name);
            SaveDataFriends();
            CounterFriendsOn();
        }

        public void ChatPrivateFriend(string name)
        {
            chat.OnInputMsg(true);
            chat.msgInput.text = "/private " + name + " ";
            OpenOrClosePanel();
        }

        public void ChatGlobalFriend()
        {
            chat.OnInputMsg(true);
            chat.msgInput.text = "/friends ";
            OpenOrClosePanel();
        }

    }
}