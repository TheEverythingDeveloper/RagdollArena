using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Lobby
{
    public class LobbyBodyPartManager : MonoBehaviour
    {
        public int selectedBodyPart;
        private Button[] _allBodyPartButtons;

        private void Awake()
        {
            _allBodyPartButtons = GetComponentsInChildren<Button>();
            _allBodyPartButtons[0].animator.SetTrigger("ChangeSelection");
        }

        public void SelectBodyPart(int ID)
        {
            if (!_canSelectBodyPart) return;

            _allBodyPartButtons[selectedBodyPart].animator.SetTrigger("ChangeSelection");
            if (selectedBodyPart != ID)
            {
                selectedBodyPart = ID;
                _allBodyPartButtons[ID].animator.SetTrigger("ChangeSelection");
            }
            _canSelectBodyPart = false;
            StartCoroutine(SelectBodyPartCoroutine());
        }

        bool _canSelectBodyPart = true;
        private IEnumerator SelectBodyPartCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _canSelectBodyPart = true;
        }
    }
}
