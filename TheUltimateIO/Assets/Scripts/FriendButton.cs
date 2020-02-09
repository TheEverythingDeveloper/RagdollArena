using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendButton : MonoBehaviour
{
    FriendSystem _friendSystem;
    public string namePlayer;
    public bool friend;

    public GameObject friendButtons, noFriendButtons;

    public void StartButton(FriendSystem fs, Friends f)
    {
        _friendSystem = fs;
        CheckFriend(f);
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
}
