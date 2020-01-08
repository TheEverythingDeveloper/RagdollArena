using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameUI
{
    public class TeamPanel : MonoBehaviour
    {
        private Image _background;
        [SerializeField] private TextMeshProUGUI[] _allMembers;
        public int membersAmount;
        public int teamID;
        [SerializeField] private Color[] _teamBackgroundColors;
        [SerializeField] private Image _coreLifebar;
        
        public void UpdatePanel()
        {
            _background = GetComponent<Image>();
            for (int i = 0; i < _allMembers.Length; i++) //recorrer el array y desactivar los que no estan
                _allMembers[i].gameObject.SetActive(false);
            for (int i = 0; i < membersAmount; i++) //recorrer el array y desactivar los que no estan
                _allMembers[i].gameObject.SetActive(true);

            _background.color = _teamBackgroundColors[teamID];
            _coreLifebar.color = _teamBackgroundColors[teamID] * 1.5f;
        }

        public void UpdateMemberData(int ID, string name) => _allMembers[ID].text = name;
        public void UpdateCoreLifebar(float amount01) => _coreLifebar.fillAmount = amount01;
    }
}
