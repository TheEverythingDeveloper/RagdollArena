using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class TeamsTable : MonoBehaviour
    {
        private List<TeamPanel> _allTeamPanels = new List<TeamPanel>();
        private List<string> _allPlayersNames = new List<string>();
        public Dictionary<int, int[]> _teamCombinations = new Dictionary<int, int[]>();
        private int playersAmount;
        [SerializeField] private TeamPanel _teamPanelPrefab;

        private void Awake()
        {
            _teamCombinations.Add(1, new int[]{1,1}); //1 jugador, 1 panel de 1 persona
            _teamCombinations.Add(2, new int[]{2,1}); //2 jugadores, 2 paneles de 1 persona
            _teamCombinations.Add(3, new int[]{3,1}); //3 jugadores, 3 paneles de 1 persona
            _teamCombinations.Add(4, new int[]{2,2}); //4 jugadores, 2 paneles de 2 personas
            _teamCombinations.Add(6, new int[]{2,3}); //6 jugadores, 2 paneles de 3 personas
            _teamCombinations.Add(8, new int[]{4,2}); //8 jugadores, 4 paneles de 2 personas
            _teamCombinations.Add(9, new int[]{3,3}); //9 jugadores, 3 paneles de 3 personas
            _teamCombinations.Add(12, new int[]{4,3}); //12 jugadores, 4 paneles de 3 personas
            _teamCombinations.Add(16, new int[]{4,4}); //16 jugadores, 4 paneles de 4 personas
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                //add player
                AddPlayer(Random.Range(0, 100000).ToString());
            }
        }

        public void AddPlayer(string name)
        {
            _allPlayersNames.Add(name);
            playersAmount++;

            //destruir todos los paneles
            foreach (var x in _allTeamPanels)
                Destroy(x.gameObject);
            _allTeamPanels.Clear();

            int resultPlayersAmount = AnalyzeTeamOrganization(playersAmount);
            
            //crear paneles segun la estructura nueva
            CreatePanelsWithStructure(_allPlayersNames, _teamCombinations[resultPlayersAmount][0], _teamCombinations[resultPlayersAmount][1]);
        }

        public int AnalyzeTeamOrganization(int actualPlayersAmount)
        {
            if (_teamCombinations.ContainsKey(actualPlayersAmount))
                return actualPlayersAmount;
            return AnalyzeTeamOrganization(actualPlayersAmount - 1);
        }

        public void CreatePanelsWithStructure(List<string> allPlayersNames, int panelsAmount, int playersPerPanelAmount)
        {
            int actualNameID = 0;
            for (int i = 0; i < panelsAmount; i++)
            {
                var panel = Instantiate(_teamPanelPrefab, transform);

                panel.membersAmount = playersPerPanelAmount;
                panel.teamID = i;
                panel.UpdatePanel();

                for (int j = 0; j < playersPerPanelAmount; j++)
                {
                    actualNameID++;
                    panel.UpdateMemberData(j, allPlayersNames[actualNameID - 1]);
                }

                _allTeamPanels.Add(panel);
            }
        }
    }
}
