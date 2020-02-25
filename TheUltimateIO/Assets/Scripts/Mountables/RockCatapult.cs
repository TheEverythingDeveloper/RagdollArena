using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RockCatapult : MonoBehaviourPun
{
    public Rigidbody rb;
    public GameObject model;
    public float secondsWaitTime;
    public float damage;
    public float distanceDamage;
    public LayerMask layersExploit;
    public ParticleSystem particles;
    public AudioSource sound;
    bool _exploit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_exploit) return;

        if (collision.gameObject.layer == 16 || collision.gameObject.layer == 9)
        {
            _exploit = true;
            rb.isKinematic = true;
            ExploitRock();
        }
    }

    void ExploitRock()
    {
        photonView.RPC("RPCExploitRock", RpcTarget.All);

        Collider[] hits = Physics.OverlapSphere(transform.position, distanceDamage, 1 << 16);

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].GetComponent<IDamageable>().Damage(transform.position, damage);
        }

        StartCoroutine(WaitTimeDestroy());
    }

    [PunRPC] void RPCExploitRock()
    {
        model.SetActive(false);
        particles.Play();
        sound.Play();
    }

    IEnumerator WaitTimeDestroy()
    {
        yield return new WaitForSeconds(secondsWaitTime);
        PhotonNetwork.Destroy(gameObject);
    }
}
