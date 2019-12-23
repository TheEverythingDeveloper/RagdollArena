using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Gamemodes
{
    public class GameModePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _gamemodeText;

        public void SetGameModeText(string gameModeText)
        {
            _gamemodeText.text = gameModeText;
        }
    }
}
