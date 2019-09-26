using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        PhotonNetwork.Instantiate("Character",
            new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f)), 
            Quaternion.identity);
    }
}
