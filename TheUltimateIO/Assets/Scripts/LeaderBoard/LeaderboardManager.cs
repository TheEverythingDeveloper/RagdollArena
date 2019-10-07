using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Leaderboard
{
    public class LeaderboardManager
    {
        LevelManager _lvlMng;
        public Dictionary<string, UserData> allUserData = new Dictionary<string, UserData>();
        public LeaderboardTable table;

        public LeaderboardManager(LevelManager lvlMng)
        {
            _lvlMng = lvlMng;
        }

        public void UpdateUserPoints(string newUserNickname, int addedPoints)
        {
            if (allUserData.ContainsKey(newUserNickname))
            {
                allUserData[newUserNickname].points += addedPoints;
            }
            else
            {
                allUserData.Add(newUserNickname, new UserData(newUserNickname, addedPoints));
            }
            Debug.Log(allUserData[newUserNickname].nickname + " :::: " + allUserData[newUserNickname].points);

            UpdateOrder();
        }

        private void UpdateOrder()
        {
            allUserData = allUserData.OrderByDescending(x => x.Value.points).
                                      ToDictionary(x => x.Value.nickname, y => y.Value);

            //de aca llamar a rpc del levelmanager que le avise a los leaderboardmanagers que actualicen las tablas.
            string[] allNicknames = allUserData.Select(x => x.Value.nickname).ToArray();
            int[] allPoints = allUserData.Select(x => x.Value.points).ToArray();
            _lvlMng.UpdateLeaderboardTables(allNicknames, allPoints);
        }

        public void UpdateTableInfo(string[] nicknames, int[] points)
        {
            Debug.Log("no existe tabla" + (table != null));
            if (!table) return;
            table.UpdateTable(nicknames, points);
        }

        public void DebugTopRanking()
        {
            allUserData.OrderByDescending(x => x.Value.points).Select(x =>
            {
                Debug.Log(x.Value.nickname + " :::: " + x.Value.points);
                return x;
            }).ToList();
        }

        public void ShufflePoints()
        {
            allUserData.Select(x =>
            {
                x.Value.points = Random.Range(0, 1000);
                return x;
            }).ToArray();

            UpdateOrder();
        }
    }
}
