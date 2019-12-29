using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using Character;

public class Controller : MonoBehaviourPun
{
    private void Start()
    {
        Server.Instance.AddPlayer(photonView.Controller, FindObjectOfType<CharacterModel>());
    }
}
