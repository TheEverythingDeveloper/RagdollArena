using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Character;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using GameUI;

namespace Character 
{
    public class CharacterWeapon : MonoBehaviourPun
    {
        Func<int> weaponActive;
        CharacterStats _characterStats;
        [HideInInspector] public CharacterModel characterModel;
        WeaponsAndStatsManager _weaponMng;
        private bool[] _allAttacksCd = new bool[3];

        Coroutine _attack;

        GameObject _capsule;

        public LayerMask layerMask;
        private void Awake()
        {
            _weaponMng = FindObjectOfType<WeaponsAndStatsManager>();
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
                StartCoroutine(AttackCoroutine());
                
        }

        int Shield()
        {
            if (_allAttacksCd[0]) return 0;
            _allAttacksCd[0] = true;
            Debug.Log("<color=blue> Se posiciono en modo defensivo con el escudo. </color>");

            //TODO: Funcionamiento del Escudo

            return 0;
        }

        int Sword()
        {
            if (_allAttacksCd[1]) return 1;
            _allAttacksCd[1] = true;
            Debug.Log("<color=blue> Se ataco con la espada. </color>");

            RaycastHit hit;
            if (Physics.Raycast(_capsule.transform.position, _capsule.transform.up, out hit, _characterStats.verticalDistAttack, layerMask))
                hit.collider.gameObject.GetComponent<Damageable>().Damage(_characterStats.damageAttack);

            return 1;
        }

        int Bow()
        {
            if (_allAttacksCd[2]) return 2;
            _allAttacksCd[2] = true;
            Debug.Log("<color=blue> Se ataco con el arco. </color>");

            var arr = PhotonNetwork.Instantiate("Arrow", characterModel.pelvisRb.position, characterModel.pelvisRb.transform.rotation);
            arr.transform.Rotate(Vector3.right, -90);
            arr.GetComponent<Arrow>().ownerWeapon = this;

            return 2;
        }

        IEnumerator AttackCoroutine()
        {
            int attackID = weaponActive();
            yield return new WaitForSeconds(_characterStats.delayMeleeAttackInSeconds);
            _allAttacksCd[attackID] = false;
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

            if (weaponActive == Shield)
                _weaponMng.UpdateWeapon(0);
            else if (weaponActive == Sword)
                _weaponMng.UpdateWeapon(1);
            else if (weaponActive == Bow)
                _weaponMng.UpdateWeapon(2);
        }

        private void OnDrawGizmos()
        {
            if (_capsule == null) return;
            Gizmos.DrawLine(_capsule.transform.position, _capsule.transform.up * _characterStats.verticalDistAttack); 
        }
    } 
}
