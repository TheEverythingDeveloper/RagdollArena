using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        var user = PhotonNetwork.Instantiate("User",
            new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f)), 
            Quaternion.identity);
        user.GetComponentInChildren<Character3DUI>().photonView.RPC("RPCUpdateNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }
}
