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

        Action weaponActive;
        Action weaponOff;
        CharacterModel _characterModel;
        GameCanvas _gameCanvas;
        WeaponsAndStatsUIManager _weaponUIMng;

        private bool[] _allAttacksCd = new bool[3];

        bool controlsActive = true;

        public CharacterWeapon(CharacterModel cm, WeaponsAndStatsUIManager weaponUIMng, WeaponsManager weaponsManager, Chat chat, GameCanvas canvas)
        {
            _characterModel = cm;
            _weaponUIMng = weaponUIMng;
            weaponsMng = weaponsManager;
            weaponActive = Sword;
            weaponOff = delegate { };
            _gameCanvas = canvas;
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
                weaponActive();
            if (Input.GetMouseButtonUp(0))
                weaponOff();
        }

        public void ArtificialFixedUpdate() { }

        public void ArtificialLateUpdate() { }

        void ChatActive(bool active)
        {
            controlsActive = active;
        }
        void Shield()
        {
            Debug.Log("<color=blue> Se posiciono en modo defensivo con el escudo. </color>");

            _characterModel.photonView.RPC("RPCActiveShield", RpcTarget.AllBuffered, true);

        }

        void ShieldUp()
        {
            _characterModel.photonView.RPC("RPCActiveShield", RpcTarget.AllBuffered, false);
        }

        void Sword()
        {
            if (_allAttacksCd[1]) return;
            _allAttacksCd[1] = true;
            Debug.Log("<color=blue> Se ataco con la espada. </color>");

            _characterModel.photonView.RPC("RPCAnimSword", RpcTarget.All);

            RaycastHit hit;
            if (Physics.Raycast(weaponsMng.transform.position, -weaponsMng.transform.forward, out hit, _characterModel.characterStats.verticalDistAttack, _characterModel.layerMaskWeaponDamage))
                hit.collider.gameObject.GetComponent<Damageable>().Damage(weaponsMng.transform.position, _characterModel.characterStats.damageAttack);

            _characterModel.StartCoroutine(CooldownAttack(1));
        }  

        void Bow()
        {
            if (_allAttacksCd[2]) return;
            _allAttacksCd[2] = true;
            Debug.Log("<color=blue> Se ataco con el arco. </color>");

            _characterModel.StartCoroutine(BowCoroutine());

            _characterModel.StartCoroutine(CooldownAttack(2));
        }

        

        private Arrow _lastArrow;
        IEnumerator BowCoroutine()
        {
            if (_lastArrow == null)
                SpawnArrow();
            _characterModel.photonView.RPC("RPCAnimBow", RpcTarget.All);
            yield return new WaitForSeconds(_characterModel.characterStats.delayAnimBowInSeconds);
            _lastArrow.ThrowArrow();
        }

        private void SpawnArrow()
        {
            _lastArrow = PhotonNetwork.Instantiate("Arrow", weaponsMng.arrowSpawnTransform.position, weaponsMng.arrowSpawnTransform.rotation).GetComponent<Arrow>();
            _lastArrow.photonView.RPC("RPCUpdateWeaponColors", RpcTarget.All, _teamColor.r, _teamColor.g, _teamColor.b);
            _lastArrow.ownerWeapon = _characterModel;
            _lastArrow.transform.parent = weaponsMng.arrowSpawnTransform;
        }

        IEnumerator CooldownAttack(int attackID)
        {
            var WaitForEndOfFrame = new WaitForEndOfFrame();
            var counter = _characterModel.characterStats.delayAttackInSeconds[attackID];
            _gameCanvas.ActiveCooldown(true);

            while (counter > 0)
            {
                counter -= Time.deltaTime;
                _gameCanvas.ChangeCooldown(counter / _characterModel.characterStats.delayAttackInSeconds[attackID]);
                yield return WaitForEndOfFrame;
            }
            _gameCanvas.ActiveCooldown(false);
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
