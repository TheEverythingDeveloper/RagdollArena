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

        private void Awake()
        {
           // UpdatePanel();
        }

        public void UpdatePanel()
        {
            _background = GetComponent<Image>();
            for (int i = (4 - membersAmount); i > 0; i--) //recorrer el array y desactivar los que no estan
                _allMembers[i].gameObject.SetActive(false);

            _background.color = _teamBackgroundColors[teamID];
        }

        public void UpdateMemberData(int ID, string name)
        {
            _allMembers[ID].text = name;
        }
    }
}
