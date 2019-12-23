using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using System.Linq;

namespace Leaderboard
{
    public class LeaderboardTable : MonoBehaviour
    {
        public GameObject userRankPrefab;
        public List<UserRank> allTopRank = new List<UserRank>(10); //capacidad de 10 en este array dinamico

        public void NewPlayer()
        {
            var userRankGo = Instantiate(userRankPrefab, transform);
            allTopRank.Add(userRankGo.GetComponent<UserRank>());
            userRankGo.name = "User Rank " + allTopRank.Count;
        }

        public void RemovePlayer()
        {
            int lastIndex = allTopRank.Count - 1;
            Destroy(allTopRank[lastIndex]);
            allTopRank.RemoveAt(lastIndex);
        }

        private string _actualFirstRankNickname;
        public void UpdateTable(string[] allNicknames, int[] allPoints)
        {
            int amountDifference = allNicknames.Length - allTopRank.Count;
            while (amountDifference != 0)
            {
                if (amountDifference > 0)
                {
                    NewPlayer();
                    amountDifference--;
                }
                else
                {
                    RemovePlayer();
                    amountDifference++;
                }
            }
            Debug.Log("llego a actualizarse");

            for (int i = 0; i < allNicknames.Length; i++)
            {
                allTopRank[i].UpdateUserRank(i + 1, allNicknames[i], allPoints[i]);
            }

            if (_actualFirstRankNickname == allNicknames[0])
                return;

            Debug.Log("bla");
            //conseguir todos los charactermodel
            var allCharacters = FindObjectsOfType<CharacterModel>();

            //conseguir el charactermodel con el name del antiguo rank 1
            var character1 = allCharacters.FirstOrDefault(x => x.name == _actualFirstRankNickname);
            if (character1 != null)
                character1.Crowned(false); //sacarle la corona

            _actualFirstRankNickname = allNicknames[0];
            //conseguir el charactermodel con el name del rank 1
            var character2 = allCharacters.FirstOrDefault(x => x.name == allNicknames[0]);
            if (character2 != null)
                character2.Crowned(true); //ponerle la corona
        }
    }
}
