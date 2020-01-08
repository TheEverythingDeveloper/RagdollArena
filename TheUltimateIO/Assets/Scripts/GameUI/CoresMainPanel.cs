using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class CoresMainPanel : MonoBehaviour
    {
        [SerializeField] private Image[] _allLifebars = new Image[4];

        public void UpdateCorePanelStructure(int amountOfTeams)
        {
            for (int i = 0; i < _allLifebars.Length; i++) //recorrer el array y desactivar los que no estan
                _allLifebars[i].transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < amountOfTeams; i++) //recorrer el array y desactivar los que no estan
                _allLifebars[i].transform.parent.gameObject.SetActive(true);
        }

        public void UpdateCoreLifebar(int teamID, float newHP)
        {
            _allLifebars[teamID].fillAmount = newHP;
        }
    }
}
