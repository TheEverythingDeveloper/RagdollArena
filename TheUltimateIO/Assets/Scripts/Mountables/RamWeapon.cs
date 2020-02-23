using System.Collections;
using UnityEngine;
using Character;
using Photon.Pun;

public class RamWeapon : WeaponVehicle
{
    public Vector3 forward;


    public override void Attack()
    {
        if (_coroutineAttack == null)
        {
            _attack = true;
            _coroutineAttack = StartCoroutine(AttackRam());
        }
    }

    IEnumerator AttackRam()
    {
        var WaitForEndOfFrame = new WaitForEndOfFrame();
        while (_attack)
        {
            transform.localPosition += forward * speedAttack * Time.deltaTime;

            if (transform.localPosition.z >= limit) _attack = false;

            yield return WaitForEndOfFrame;
        }

        while (transform.localPosition.z > _initialPosition.z)
        {
            transform.localPosition -= forward * speedReturn * Time.deltaTime;

            yield return WaitForEndOfFrame;
        }

        _coroutineAttack = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_attack) return;

        if (other.gameObject.layer == Layers.CHARACTER)
            other.gameObject.GetComponent<CharacterModel>().Damage(transform.position, damage);
        else if (other.gameObject.layer == Layers.CONSTRUCTIONBLOCK)
        {
            other.gameObject.GetComponent<SpawnedCube>().Damage(transform.position, damage);
            PhotonNetwork.Instantiate("ParticlesRam", transform.position, transform.rotation);
            _attack = false;
        }
    }
}
