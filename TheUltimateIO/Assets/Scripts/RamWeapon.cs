using System.Collections;
using UnityEngine;
using Character;
using Photon.Pun;
public class RamWeapon : WeaponVehicle
{
    public override void ActiveWeapon()
    {
        if (_coroutineAttack == null)
        {
            Debug.LogError("ATTACK");
            _attack = true;
            _coroutineAttack = StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        var WaitForEndOfFrame = new WaitForEndOfFrame();
        while (_attack)
        {
            transform.localPosition += new Vector3(0,0,1) * speedAttack * Time.deltaTime;

            if (transform.localEulerAngles.z >= maxPosZ) _attack = false;

            yield return WaitForEndOfFrame;
        }

        while (transform.localEulerAngles.z > _initialPosition.z)
        {
            transform.localPosition -= transform.forward * speedReturn * Time.deltaTime;

            yield return WaitForEndOfFrame;
        }

        _coroutineAttack = null;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (!_attack) return;

        if (collision.gameObject.layer == Layers.CHARACTER)
            collision.gameObject.GetComponent<CharacterModel>().Damage(transform.position, damage);
        else if (collision.gameObject.layer == Layers.CONSTRUCTIONBLOCK)
        {
            collision.gameObject.GetComponent<SpawnedCube>().Damage(transform.position, damage);
            PhotonNetwork.Instantiate("ParticlesRam", transform.position, transform.rotation);
            _attack = false;
        }
    }
}
