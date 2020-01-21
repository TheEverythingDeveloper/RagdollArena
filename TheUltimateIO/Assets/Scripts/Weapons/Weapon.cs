using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        public void EnableFeedback(bool active)
        {
            //TODO: Feedback de aparicion del arma. (Que se agrande rapido, que tire particulas alrededor, algo asi, que haga sonidos).
        }

        public void UpdateWeaponColors(float r, float g, float b)
        {
            GetComponentInChildren<Renderer>().material.color = new Color(r, g, b, 1f);
        }
    }
}
