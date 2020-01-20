using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Character;

namespace Character {
    public class CharacterWeapon : MonoBehaviour
    {
        Action weaponActive;
        CharacterStats _characterStats;
        CharacterModel _characterModel;

        Coroutine _swordAttack;

        GameObject _capsule;

        public LayerMask layerMask;
    private void Awake()
        {
            weaponActive = Construct;
            _characterStats = GetComponent<CharacterStats>();
            _characterModel = GetComponent<CharacterModel>();

            _capsule = _characterModel._ragdollCapsule;
        }

        private void FixedUpdate()
        {
            weaponActive();
            ChangeWeapon();
        }

        void Construct()
        {

        }

        void Sword()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_swordAttack == null)
                    _swordAttack = StartCoroutine(SwordAttack());
            }
        }

        IEnumerator SwordAttack()
        {/*
            Collider[] col = Physics.OverlapBox(_myModel.transform.position + (_myModel.transform.forward * _characterStats.initialDistAttack),
                                                (_myModel.transform.forward * _characterStats.verticalDistAttack) + 
                                                (_myModel.transform.right * _characterStats.horizontalDistAttack), _myModel.rotation, Layers.DAMAGEABLE);*/

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

        }

        void ChangeWeapon()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                weaponActive = Construct;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                weaponActive = Sword;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                weaponActive = Bow;
        }

        private void OnDrawGizmos()
        {/*
            Gizmos.DrawCube(_myModel.transform.position + (_myModel.transform.forward * _characterStats.initialDistAttack),
                                                (_myModel.transform.forward * _characterStats.verticalDistAttack) + 
                                                (_myModel.transform.right * _characterStats.horizontalDistAttack));
                                                */
            Gizmos.DrawLine(_capsule.transform.position, _capsule.transform.up * _characterStats.verticalDistAttack); 
        }
    } }
