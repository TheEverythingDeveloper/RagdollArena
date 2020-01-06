using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GameUI;

namespace GameUI
{
    public class GameCanvas : MonoBehaviour
    {
        [Tooltip("Panel de conteo antes de empezar la partida, es el gameobject padre de todos los relacionados al conteo")] [SerializeField] private GameObject _counterPanel;
        [Tooltip("Panel de respawn cuando el jugador esta muerto y tiene que elegir donde respawnear")] [SerializeField] private GameObject _respawnHUD;
        [Tooltip("Texto del conteo. 5..4..3..2..1..GO!")] [SerializeField] private TextMeshProUGUI _counterText;
        [Tooltip("Texto que solo le va a aparecer al server para iniciar la partida")] [SerializeField] private TextMeshProUGUI _pressEnterToStartText;

        private void Awake()
        {
            SwitchCounterPanel(false);
            SwitchEnterToStartText(false);
        }
        public void CounterUpdate(int time)
        {
            //TODO: Que ese numero tenga animacion y vaya cambiando de color mientras menos tiempo quede. Puede tambien haber una barra.
            //TODO: Sonido cada "tick"
            _counterText.text = time.ToString();
            if (time < 1) _counterText.text = "GO!";
        }
        public void SwitchCounterPanel(bool active) => _counterPanel.SetActive(active);
        public void SwitchRespawnHUD(bool active) => _respawnHUD.SetActive(active);
        public void SwitchEnterToStartText(bool active) => _pressEnterToStartText.gameObject.SetActive(active);
    }
}
