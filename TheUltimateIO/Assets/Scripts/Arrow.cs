using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon;
using System.Linq;
using Character;

public class Arrow : MonoBehaviourPun
{
    public float speed;
    public float damage;
    public float lifeTime;
    public int teamID;
    [HideInInspector] public CharacterWeapon ownerWeapon;
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
        if (!ownerWeapon.characterModel.owned) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            DestroyArrow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!thrownArrow) return;
        if (ownerWeapon == null) return;
        if (!ownerWeapon.characterModel.owned) return;
        if (other.gameObject.layer != 17) return;

        other.GetComponent<Damageable>().Damage(transform.position, damage);
        DestroyArrow();
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
