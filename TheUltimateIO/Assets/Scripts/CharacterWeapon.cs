using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Character;
using Photon.Pun;
using Photon.Realtime;
using Photon;

namespace Character 
{
    public class CharacterWeapon : MonoBehaviourPun
    {
        Action weaponActive;
        CharacterStats _characterStats;
        [HideInInspector] public CharacterModel characterModel;

        Coroutine _swordAttack;

        GameObject _capsule;

        public LayerMask layerMask;
        private void Awake()
        {
            weaponActive = Sword;
            _characterStats = GetComponent<CharacterStats>();
            characterModel = GetComponent<CharacterModel>();

            _capsule = characterModel._ragdollCapsule;
        }

        private void Update()
        {
            if (!characterModel.owned) return;

            if (Input.GetKeyDown(KeyCode.Q))
                SelectWeapon(false);
            else if (Input.GetKeyDown(KeyCode.E))
                SelectWeapon(true);

            if (Input.GetMouseButtonDown(0))
                weaponActive();
        }

        void Shield()
        {
            Debug.Log("<color=blue> Se posiciono en modo defensivo con el escudo. </color>");
        }

        void Sword()
        {
            Debug.Log("<color=blue> Se ataco con la espada. </color>");
            if (Input.GetMouseButtonDown(0))
            {
                if (_swordAttack == null)
                    _swordAttack = StartCoroutine(SwordAttack());
            }
        }

        IEnumerator SwordAttack()
        {
            RaycastHit hit;
            if(Physics.Raycast(_capsule.transform.position, _capsule.transform.up, out hit, _characterStats.verticalDistAttack, layerMask))
            {
                Debug.LogError("Toque con: " + hit.collider.name);
                hit.collider.gameObject.GetComponent<Damageable>().Damage(_characterStats.damageAttack);
            }
            /*
            var objDamageables = col.Select(x => x.GetComponent<Damageable>());

            foreach (var item in objDamageables)
            {
                item.Damage(_characterStats.damageAttack);
            }
            */
            yield return new WaitForSeconds(_characterStats.delayMeleeAttackInSeconds);

            _swordAttack = null;
        }

        void Bow()
        {
            Debug.Log("<color=blue> Se ataco con el arco. </color>");

            var arr = PhotonNetwork.Instantiate("Arrow", characterModel.pelvisRb.position, characterModel.pelvisRb.transform.rotation);
            arr.transform.Rotate(Vector3.right, -90);
            arr.GetComponent<Arrow>().ownerWeapon = this;
        }

        private void SelectWeapon(bool right) //cadena de ifs porque no hay necesidad de hacerlo mas complejo al ser solo 3, yay!
        {
            if (right)
            {
                if (weaponActive == Shield)
                    weaponActive = Sword;
                else if (weaponActive == Bow)
                    weaponActive = Shield;
                else
                    weaponActive = Bow;
            }
            else
            {
                if (weaponActive == Shield)
                    weaponActive = Bow;
                else if (weaponActive == Sword)
                    weaponActive = Shield;
                else
                    weaponActive = Sword;
            }
        }

        private void OnDrawGizmos()
        {
            if (_capsule == null) return;
            Gizmos.DrawLine(_capsule.transform.position, _capsule.transform.up * _characterStats.verticalDistAttack); 
        }
    } 
}
