using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public class UserData
{
    public string nickname;
    public int points;
    public UserData(string actualNickname, int initialPoints)
    {
        nickname = actualNickname;
        points = initialPoints;
    }
}
