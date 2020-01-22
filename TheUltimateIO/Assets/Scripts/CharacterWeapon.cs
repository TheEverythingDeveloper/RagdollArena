using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using GameUI;
using Weapons;

namespace Character 
{
    public class CharacterWeapon : MonoBehaviourPun
    {
        Func<int> weaponActive;
        CharacterStats _characterStats;
        [HideInInspector] public CharacterModel characterModel;
        WeaponsAndStatsUIManager _weaponUIMng;
        private WeaponsManager _weaponsMng;
        private bool[] _allAttacksCd = new bool[3];

        GameObject _capsule;
        public LayerMask layerMask;

        private void Awake()
        {
            _weaponUIMng = FindObjectOfType<WeaponsAndStatsUIManager>();
            _weaponsMng = GetComponentInChildren<WeaponsManager>();
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
                hit.collider.gameObject.GetComponent<Damageable>().Damage(transform.position, _characterStats.damageAttack);

            return 1;
        }

        int Bow()
        {
            if (_allAttacksCd[2]) return 2;
            _allAttacksCd[2] = true;
            Debug.Log("<color=blue> Se ataco con el arco. </color>");

            StartCoroutine(BowCoroutine());

            return 2;
        }

        private Arrow _lastArrow;
        IEnumerator BowCoroutine()
        {
            if (_lastArrow == null)
                SpawnArrow();
            _lastArrow.ThrowArrow();
            yield return new WaitForSeconds(_characterStats.delayMeleeAttackInSeconds - 0.4f);
            SpawnArrow();
        }

        private void SpawnArrow()
        {
            _lastArrow = PhotonNetwork.Instantiate("Arrow", _weaponsMng.arrowSpawnTransform.position, _weaponsMng.arrowSpawnTransform.rotation).GetComponent<Arrow>();
            _lastArrow.photonView.RPC("RPCUpdateWeaponColors", RpcTarget.All, _teamColor.r, _teamColor.g, _teamColor.b);
            _lastArrow.ownerWeapon = this;
            _lastArrow.transform.parent = _weaponsMng.arrowSpawnTransform;
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
            int selectedWeaponID = 0;
            if (weaponActive == Shield)
                selectedWeaponID = 0;
            else if (weaponActive == Sword)
                selectedWeaponID = 1;
            else if (weaponActive == Bow)
                selectedWeaponID = 2;

            _weaponUIMng.UpdateWeapon(selectedWeaponID);
            //TODO: Si ya empezo la partida que sea RpcTarget.All, y si no empezo, entonces que sea RpcTarget.AllBuffered
            photonView.RPC("RPCChangeWeapon", RpcTarget.All, selectedWeaponID); 
        }

        [PunRPC] public void RPCChangeWeapon(int selectedWeaponID)
        {
            _weaponsMng.ChangeWeapon(selectedWeaponID);
        }

        private Color _teamColor;
        public void UpdateWeaponColors(float r, float g, float b)
        {
            _weaponsMng.UpdateWeaponColors(r, g, b);
            _teamColor = new Color(r, g, b, 1f);
        }

        private void OnDrawGizmos()
        {
            if (_capsule == null) return;
            Gizmos.DrawLine(_capsule.transform.position, _capsule.transform.up * _characterStats.verticalDistAttack); 
        }
    } 
}
