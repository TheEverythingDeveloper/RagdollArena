using TMPro;
using UnityEngine;
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
        private CoresMainPanel _coreMainPanel;

        private void Awake() { _coreMainPanel = FindObjectOfType<CoresMainPanel>(); }

        public void TeamDeath()
        {
            _background.color *= 0.33f;
            _coreLifebar.color *= 0.33f;
            foreach (var x in _allMembers)
                x.color *= 0.65f;
        }

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
        public void UpdateCoreLifebar(float amount01)
        {
            _coreMainPanel.UpdateCoreLifebar(teamID, amount01);
            _coreLifebar.fillAmount = amount01;
        }
    }
}
