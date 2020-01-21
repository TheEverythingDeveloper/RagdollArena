using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameUI
{
    public class WeaponsAndStatsManager : MonoBehaviour
    {
        [SerializeField] private Image[] allImages = new Image[3];
        private Sprite[] allSprites = new Sprite[3];
        public int actualWeapon;

        private void Awake()
        {
            for (int i = 0; i < allImages.Length; i++)
                allSprites[i] = allImages[i].sprite;
        }

        public void UpdateWeapon(int ID)
        {
            actualWeapon = ID;
            allImages[0].sprite = allSprites[SelectedID(actualWeapon, false)];
            allImages[1].sprite = allSprites[actualWeapon];
            allImages[2].sprite = allSprites[SelectedID(actualWeapon, true)];

            allImages[0].color = Color.grey;
            allImages[1].color = Color.green;
            allImages[2].color = Color.grey;
        }

        private int SelectedID(int ID, bool right)
        {
            if (ID == 0) return (right ? 1 : 2);
            if (ID == 2) return (right ? 0 : 1);
            else return ID + (right ? 1 : -1);
        }
    }
}
