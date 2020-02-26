using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class CatapultPanelUI : PanelVehicles
    {
        public Image barForce;
        public Color[] colorForce;
    
        public void BarForce(float fill)
        {
            barForce.fillAmount = fill;
            var c = Color.Lerp(colorForce[0], colorForce[1], fill);
            barForce.color = c;
        }
    }
}
