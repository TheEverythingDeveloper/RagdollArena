using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Character;

namespace Character 
{
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
            weaponActive = Sword;
            _characterStats = GetComponent<CharacterStats>();
            _characterModel = GetComponent<CharacterModel>();

            _capsule = _characterModel._ragdollCapsule;
        }

        private void Update()
        {
            if (!_characterModel.owned) return;

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
        }

        private void SelectWeapon(bool right) //cadena de ifs porque no hay necesidad de hacerlo mas complejo al ser solo 3, yay!
        {
            if (right)
            {
                if (weaponActive == Shield)
                    weaponActive = Bow;
                else if (weaponActive == Bow)
                    weaponActive = Sword;
                else
                    weaponActive = Shield;
            }
            else
            {
                if (weaponActive == Shield)
                    weaponActive = Sword;
                else if (weaponActive == Sword)
                    weaponActive = Bow;
                else
                    weaponActive = Shield;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(_capsule.transform.position, _capsule.transform.up * _characterStats.verticalDistAttack); 
        }
    } 
}
