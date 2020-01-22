using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;
using Character;

namespace Lobby
{
    public class LobbyColorManager : MonoBehaviour
    {
        public Color selectedColor;
        public Color[] customizationColors;
        private Image[] _colorImages;
        private CustomizableSkinController _skinController;

        private void Awake()
        {
            _skinController = FindObjectOfType<CustomizableSkinController>();
            _colorImages = GetComponentsInChildren<Image>();
            _colorImages = _colorImages.Where(x => x.CompareTag("Color")).ToArray(); //filtrar solo los que tienen tag color para no agarrar botones

            for (int i = 0; i < _colorImages.Length - 1; i++)
            {
                _colorImages[i].color = customizationColors[i];
            }
        }

        public void ChangeColor(int ID)
        {
            selectedColor = _colorImages[ID].color;
            _colorImages[_colorImages.Length - 1].color = selectedColor;
            _skinController.UpdateColor(selectedColor);
            FindObjectOfType<CharacterBody>().UpdateAllSkinColors(selectedColor);
        }
    }
}
