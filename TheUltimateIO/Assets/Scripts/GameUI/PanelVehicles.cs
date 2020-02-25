using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class PanelVehicles : MonoBehaviour
    {
        public GameObject panelControls;
        public KeyCode keyViewControls;
        public Image barLife;

        protected bool _chatActive;

        private void Start()
        {
            FindObjectOfType<Chat>().SuscribeChat(ChangeActive);
        }

        public void ChangeActive(bool active)
        {
            _chatActive = active;
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyViewControls))
                panelControls.SetActive(true);
            else if (Input.GetKeyUp(keyViewControls))
                panelControls.SetActive(false);
        }

        public void ChangeLife(float fill) { barLife.fillAmount = fill; }
    }
}
