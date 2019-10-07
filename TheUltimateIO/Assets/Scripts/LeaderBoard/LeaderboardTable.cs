using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void UpdateTable(string[] allNicknames, int[] allPoints)
    {
        int amountDifference = allNicknames.Length - allTopRank.Count;
        while(amountDifference != 0)
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
    }
}
