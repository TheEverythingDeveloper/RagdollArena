using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class TimeBar : MonoBehaviour
    {
        public object owner;
        private const float animationDuration = 28f;
        public Image timebarImage;
        public float movingLinesOffset;

        /// <summary>
        /// Actualizar la barra de tiempo con un numero de 0 a 1
        /// </summary>
        /// <param name="time01"></param>
        public void UpdateTimebar(float time01)
        {
            timebarImage.material.SetTextureOffset("_MaskTexture", new Vector2(Mathf.Clamp(time01 - 1, -1, 0), 0));
            timebarImage.material.SetTextureOffset("_LineTexture", new Vector2(Mathf.Clamp(time01 + movingLinesOffset, -1, 1), -5.5f));
            timebarImage.material.SetFloat("_Amount", time01);
        }
    }
}
