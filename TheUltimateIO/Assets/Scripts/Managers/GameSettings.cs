using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private string _gameVersion = "0.0.1";
    public string GameVersion { get { return _gameVersion; } }
    [SerializeField] private string _nickName = "character";
    public string NickName
    {
        get
        {
            int value = Random.Range(0, 99999);
            return _nickName + value.ToString();
        }
    }
}
