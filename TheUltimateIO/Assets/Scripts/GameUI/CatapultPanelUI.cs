using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class CatapultPanelUI : PanelVehicles
    {
        public Image barForce;
        public Image rockImage, characterImage;
        public Color[] colorCharacter;
        public Color[] colorForce;

        public void ChangeAmmunition(int ammunition, bool busyContent)
        {
            rockImage.gameObject.SetActive(ammunition == 0);
            characterImage.gameObject.SetActive(ammunition != 0);

            if (ammunition != 0) ChangeUICharacter(busyContent);
        }

        public void ChangeUICharacter(bool contentPlayer)
        {
            characterImage.color = contentPlayer ? colorCharacter[1] : colorCharacter[0];
        }

        public void BarForce(float fill)
        {
            barForce.fillAmount = fill;
            var c = Color.Lerp(colorForce[0], colorForce[1], fill);
            barForce.color = c;
        }
    }
}
