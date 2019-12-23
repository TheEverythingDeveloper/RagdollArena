using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Leaderboard
{
    public class UserRank : MonoBehaviour
    {
        public TextMeshProUGUI myRankText;
        public TextMeshProUGUI myNicknameText;
        public TextMeshProUGUI myPointsText;

        public void UpdateUserRank(int rank, string nickname, int points)
        {
            myRankText.color = new Color(rank == 1 ? 1f : 0f, Mathf.Clamp(1f / rank * 2, 0.3f, 1f), 0);
            myRankText.text = rank.ToString();
            myNicknameText.text = nickname;
            myPointsText.text = points.ToString();
        }
    }
}
