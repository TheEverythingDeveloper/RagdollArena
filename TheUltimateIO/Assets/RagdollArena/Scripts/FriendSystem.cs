using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using GameUI;

public class Friends
{
    public List<string> namesFriends = new List<string>();
}
public class FriendSystem : MonoBehaviour
{
    public Friends friends;
    public TeamManager teamManager;
    public GameObject panelPlayers;
    public GameObject contentButtons;
    public FriendButton prefabButtons;
    List<FriendButton> _buttons = new List<FriendButton>();

    string _jsonSavePath;

    private void Start()
    {
        friends = new Friends();
        _jsonSavePath = Application.persistentDataPath + "/friends.json";
        LoadDataFriends();
    }

    public void OpenOrClosePanel(bool open)
    {
        panelPlayers.SetActive(open);
    }

    public void AddButtonPanel(string name)
    {
        var newButton = Instantiate(prefabButtons);
        newButton.gameObject.SetActive(true);
        newButton.transform.parent = contentButtons.transform;
        newButton.namePlayer = name;
        newButton.StartButton(this, friends);
        _buttons.Add(newButton);
    }

    public void RemoveButtonPanel()
    {

    }

    void SaveDataFriends()
    {
        string jsonData = JsonUtility.ToJson(friends, true);
        File.WriteAllText(_jsonSavePath, jsonData);
    }

    void LoadDataFriends()
    {
        friends = JsonUtility.FromJson<Friends>(File.ReadAllText(Application.persistentDataPath + "/friends.json"));
    }

}
