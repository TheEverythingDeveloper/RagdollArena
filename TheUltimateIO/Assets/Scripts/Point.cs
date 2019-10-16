using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Point : MonoBehaviourPun
{
    public int points;
    public int heightDestroy = -5;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.CHARACTER || collision.gameObject.layer == Layers.PLAYER)
        {
            if(collision.gameObject.GetComponentInParent<Character.CharacterModel>())
                collision.gameObject.GetComponentInParent<Character.CharacterModel>().AddPoint(points);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void QMonDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
