using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Character;

public class Point : MonoBehaviourPun
{
    public int points;
    public int heightDestroy = -5;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Layers.CHARACTER || collision.gameObject.layer == Layers.PLAYER || collision.gameObject.layer == Layers.MONSTER)
        {
            if(collision.gameObject.GetComponentInParent<CharacterModel>())
                collision.gameObject.GetComponentInParent<CharacterModel>().AddPoint(points);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    void QMonDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
