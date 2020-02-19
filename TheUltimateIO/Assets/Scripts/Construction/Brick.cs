using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using Character;

public class Brick : MonoBehaviourPun
{
    public int amountOfBricks;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            CharacterModel model = other.gameObject.GetComponentInParent<CharacterModel>();
            if (model != null && model.owned)
            {
                FindObjectOfType<CubeSpawner>().ConstructionPoints += amountOfBricks;
                photonView.RPC("RPCDestroyObject", RpcTarget.MasterClient);
            }
        }
    }

    [PunRPC] public void RPCDestroyObject() { PhotonNetwork.Destroy(gameObject); }
}
