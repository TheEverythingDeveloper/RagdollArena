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

    private void Update()
    {
        if (ownerWeapon == null) return;
        if (!ownerWeapon.characterModel.owned) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            DestroyArrow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ownerWeapon == null) return;
        if (!ownerWeapon.characterModel.owned) return;
        if (other.gameObject.layer != 17) return;

        other.GetComponent<Damageable>().Damage(damage);
        DestroyArrow();
    }

    private void DestroyArrow()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
