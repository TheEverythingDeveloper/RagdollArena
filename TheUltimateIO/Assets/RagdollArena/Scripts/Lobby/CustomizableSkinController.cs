using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class CustomizableSkinController : MonoBehaviour
    {
        public Material material;
        public Texture2D[] kits;
        private LobbyBodyPartManager _bodyPartManager;

        private void Awake()
        {
            OnShirtUpdate();
            OnShortsUpdate();
            OnSocksUpdate();
            _bodyPartManager = FindObjectOfType<LobbyBodyPartManager>();
        }

        public void UpdateColor(Color updateColor)
        {
            return;
            switch(_bodyPartManager.selectedBodyPart)
            {
                case 0:
                    OnColorAUpdate(updateColor);
                    break;
                case 1:
                    OnColorBUpdate(updateColor);
                    break;
                case 2:
                    OnColorCUpdate(updateColor);
                    break;
            }
        }

        public void OnColorAUpdate(Color newColor)
        {
            material.SetColor("_ColorA", newColor);
        }

        public void OnColorBUpdate(Color newColor)
        {
            material.SetColor("_ColorB", newColor);
        }

        public void OnColorCUpdate(Color newColor)
        {
            material.SetColor("_ColorC", newColor);
        }
        void OnShirtUpdate()
        {
            material.SetTexture("_Shirt", kits[0]);
        }

        void OnShortsUpdate()
        {
            material.SetTexture("_Shorts", kits[0]);
        }

        void OnSocksUpdate()
        {
            material.SetTexture("_Socks", kits[0]);
        }
    }
}
