using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon;
using System.Linq;
using Character;
using Weapons;

public class Arrow : MonoBehaviourPun
{
    public float speed;
    public float damage;
    public float lifeTime;
    public int teamID;
    [HideInInspector] public CharacterModel ownerWeapon;
    public bool thrownArrow = false;

    public void ThrowArrow()
    {
        thrownArrow = true;
        transform.parent = null;
    }

    private void Update()
    {
        if (!thrownArrow) return;
        if (ownerWeapon == null) return;
        if (!ownerWeapon.owned) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            DestroyArrow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!thrownArrow) return;
        if (ownerWeapon == null) return;
        if (!ownerWeapon.owned) return;

        if (other.gameObject.GetComponent<IDamageable>() != null)
        {
            other.gameObject.GetComponent<IDamageable>().Damage(transform.position, damage);
            DestroyArrow();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!thrownArrow) return;
        if (ownerWeapon == null) return;
        if (!ownerWeapon.owned) return;

        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            collision.gameObject.GetComponent<IDamageable>().Damage(transform.position, damage);
            DestroyArrow();
        }
    }

    [PunRPC] public void RPCUpdateWeaponColors(float r, float g, float b)
    {
        GetComponentInChildren<Renderer>().material.color = new Color(r, g, b, 1f);
    }

    private void DestroyArrow()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
