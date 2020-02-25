using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;
using GameUI;
using Weapons;

namespace Character 
{
    public class CharacterWeapon : IUpdatable
    {
        public WeaponsManager weaponsMng;

        Func<int> weaponActive;
        Action weaponOff;
        CharacterModel _characterModel;

        WeaponsAndStatsUIManager _weaponUIMng;

        private bool[] _allAttacksCd = new bool[3];

        bool controlsActive = true;

        public CharacterWeapon(CharacterModel cm, WeaponsAndStatsUIManager weaponUIMng, WeaponsManager weaponsManager, Chat chat)
        {
            _characterModel = cm;
            _weaponUIMng = weaponUIMng;
            weaponsMng = weaponsManager;
            weaponActive = Sword;
            weaponOff = delegate { };

            chat.SuscribeChat(ChatActive);
        }

        public void ArtificialUpdate()
        {
            if (!_characterModel.owned || !controlsActive) return;

            if (Input.GetKeyDown(KeyCode.Q))
                SelectWeapon(false);
            else if (Input.GetKeyDown(KeyCode.E))
                SelectWeapon(true);

            if (Input.GetMouseButtonDown(0))
                _characterModel.StartCoroutine(AttackCoroutine());
            if (Input.GetMouseButtonUp(0))
                weaponOff();
        }

        public void ArtificialFixedUpdate() { }

        public void ArtificialLateUpdate() { }

        void ChatActive(bool active)
        {
            controlsActive = active;
        }
        int Shield()
        {
            if (_allAttacksCd[0]) return 0;
            _allAttacksCd[0] = true;
            Debug.Log("<color=blue> Se posiciono en modo defensivo con el escudo. </color>");

            _characterModel.photonView.RPC("RPCActiveShield", RpcTarget.AllBuffered, true);
            return 0;
        }

        void ShieldUp()
        {
            _characterModel.photonView.RPC("RPCActiveShield", RpcTarget.AllBuffered, false);
        }

        int Sword()
        {
            if (_allAttacksCd[1]) return 1;
            _allAttacksCd[1] = true;
            Debug.Log("<color=blue> Se ataco con la espada. </color>");

            _characterModel.photonView.RPC("RPCAnimSword", RpcTarget.All);

            RaycastHit hit;
            if (Physics.Raycast(weaponsMng.transform.position, -weaponsMng.transform.forward, out hit, _characterModel.characterStats.verticalDistAttack, _characterModel.layerMaskWeaponDamage))
                hit.collider.gameObject.GetComponent<Damageable>().Damage(weaponsMng.transform.position, _characterModel.characterStats.damageAttack);

            return 1;
        }

        

        int Bow()
        {
            if (_allAttacksCd[2]) return 2;
            _allAttacksCd[2] = true;
            Debug.Log("<color=blue> Se ataco con el arco. </color>");

            _characterModel.StartCoroutine(BowCoroutine());

            return 2;
        }

        

        private Arrow _lastArrow;
        IEnumerator BowCoroutine()
        {
            if (_lastArrow == null)
                SpawnArrow();
            _characterModel.photonView.RPC("RPCAnimBow", RpcTarget.All);
            yield return new WaitForSeconds(_characterModel.characterStats.delayAnimBowInSeconds);
            _lastArrow.ThrowArrow();
            yield return new WaitForSeconds(_characterModel.characterStats.delayBowAttackInSeconds);
            SpawnArrow();
        }

        private void SpawnArrow()
        {
            _lastArrow = PhotonNetwork.Instantiate("Arrow", weaponsMng.arrowSpawnTransform.position, weaponsMng.arrowSpawnTransform.rotation).GetComponent<Arrow>();
            _lastArrow.photonView.RPC("RPCUpdateWeaponColors", RpcTarget.All, _teamColor.r, _teamColor.g, _teamColor.b);
            _lastArrow.ownerWeapon = _characterModel;
            _lastArrow.transform.parent = weaponsMng.arrowSpawnTransform;
        }

        IEnumerator AttackCoroutine()
        {
            int attackID = weaponActive();
            yield return new WaitForSeconds(_characterModel.characterStats.delayMeleeAttackInSeconds);
            _allAttacksCd[attackID] = false;
        }

        private void SelectWeapon(bool right) //cadena de ifs porque no hay necesidad de hacerlo mas complejo al ser solo 3, yay!
        {
            if (weaponsMng.constructionMode) return;

            if (right)
            {
                if (weaponActive == Shield)
                {
                    weaponOff();
                    weaponActive = Sword;
                    weaponOff = delegate { };
                }
                else if (weaponActive == Bow)
                {
                    weaponActive = Shield;
                    weaponOff = ShieldUp;
                }
                else
                {
                    weaponActive = Bow;
                    weaponOff = delegate { };
                }
            }
            else
            {
                if (weaponActive == Shield)
                {
                    weaponOff();
                    weaponActive = Bow;
                    weaponOff = delegate { };
                }
                else if (weaponActive == Sword)
                {
                    weaponActive = Shield;
                    weaponOff = ShieldUp;
                }
                else
                {
                    weaponActive = Sword;
                    weaponOff = delegate { };
                }
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
            _characterModel.photonView.RPC("RPCChangeWeapon", RpcTarget.All, selectedWeaponID); 
        }

        private Color _teamColor;
        public void UpdateWeaponColors(float r, float g, float b)
        {
            weaponsMng.UpdateWeaponColors(r, g, b);
            _teamColor = new Color(r, g, b, 1f);
        }

    } 
}
