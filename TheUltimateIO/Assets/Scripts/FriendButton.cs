using UnityEngine;
using TMPro;

public class FriendButton : MonoBehaviour
{
    FriendSystem _friendSystem;
    public string namePlayer;
    public TextMeshProUGUI textNamePlayer;
    public bool friend;

    public GameObject friendButtons, noFriendButtons, panelRemoveFriend;
    bool _panelRemoveActive;

    public void StartButton(FriendSystem fs, Friends f)
    {
        _friendSystem = fs;
        textNamePlayer.text = namePlayer;
        CheckFriend(f);
    }

    public void AddFriend()
    {
        friend = true;
        _friendSystem.AddFriend(namePlayer);
        UpdateButton();
    }

    public void ChatPrivateFriend()
    {
        _friendSystem.ChatPrivateFriend(namePlayer);
    }

    public void ChatGlobalFriend()
    {
        _friendSystem.ChatGlobalFriend();
    }

    public void RemoveFriend(bool remove)
    {
        if (remove)
        {
            _friendSystem.RemoveFriend(namePlayer);
            friend = false;
            UpdateButton();
        }

        ClosePanelRemoveFriend();
    }
    public void CheckFriend(Friends friends)
    {
        friend = friends.namesFriends.Contains(namePlayer);
        UpdateButton();
    }

    void UpdateButton()
    {
        friendButtons.SetActive(friend);
        noFriendButtons.SetActive(!friend);
    }

    public void OpenPanelRemoveFriend()
    {
        _panelRemoveActive = true;
        _friendSystem.panelsRemoveActive.Add(ClosePanelRemoveFriend);
        panelRemoveFriend.SetActive(true);
    }

    public void ClosePanelRemoveFriend()
    {
        _panelRemoveActive = false;
        _friendSystem.panelsRemoveActive.Remove(ClosePanelRemoveFriend);
        panelRemoveFriend.SetActive(false);
    }
}
