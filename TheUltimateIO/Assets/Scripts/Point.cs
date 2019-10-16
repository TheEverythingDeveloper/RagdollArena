using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Point : MonoBehaviourPun
{
    public int points;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.CHARACTER || collision.gameObject.layer == Layers.PLAYER)
        {
            collision.gameObject.GetComponentInParent<Character.CharacterModel>().AddPoint(points);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
