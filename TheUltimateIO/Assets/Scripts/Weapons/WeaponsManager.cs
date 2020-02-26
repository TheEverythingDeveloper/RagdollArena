using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

namespace Weapons
{
    public class WeaponsManager : MonoBehaviour
    {
        [SerializeField] private Weapon[] _allFrontWeapons = new Weapon[3];
        [SerializeField] private Weapon[] _allBackWeapons = new Weapon[3];
        public AudioManager[] allSoundsWeapons = new AudioManager[3];
        private Weapon _actualFrontWeapon;
        private Weapon _actualBackWeapon;
        public Transform arrowSpawnTransform;
        public bool constructionMode;
        public GameObject shieldProtect;
        private void Awake()
        {
            ChangeWeapon(1);
        }

        public void ActiveShield(bool active)
        {
            shieldProtect.SetActive(active);
        }

        public void ConstructionMode(bool show)
        {
            constructionMode = show;
            _actualBackWeapon.gameObject.SetActive(show);
            _actualFrontWeapon.gameObject.SetActive(!show);
        }

        public void ChangeWeapon(int weaponID)
        {
            foreach (var x in _allFrontWeapons)
                x.gameObject.SetActive(false);

            _actualFrontWeapon = _allFrontWeapons[weaponID];
            _actualFrontWeapon.gameObject.SetActive(true);
            _actualFrontWeapon.EnableFeedback(true);

            foreach (var x in _allBackWeapons)
                x.gameObject.SetActive(true);

            _actualBackWeapon = _allBackWeapons[weaponID];
            _actualBackWeapon.gameObject.SetActive(false);
            _actualBackWeapon.EnableFeedback(false);
        }

        public void UpdateWeaponColors(float r, float g, float b)
        {
            foreach (var x in _allFrontWeapons)
                x.UpdateWeaponColors(r, g, b);

            foreach (var x in _allBackWeapons)
                x.UpdateWeaponColors(r, g, b);
        }
    }
}
