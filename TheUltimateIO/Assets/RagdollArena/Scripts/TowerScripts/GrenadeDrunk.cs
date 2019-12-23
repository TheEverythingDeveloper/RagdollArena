using UnityEngine;
using Photon.Pun;

public class GrenadeDrunk : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var drunk = collision.gameObject.GetComponentInParent<IDrunk>();
        if (drunk != null)
        {
            drunk.DrunkEffectActive();
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
