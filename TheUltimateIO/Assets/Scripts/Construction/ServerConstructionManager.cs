using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Construction
{
    public class ServerConstructionManager : MonoBehaviour
    {
        public List<ConstructionPlan> _allConstructions = new List<ConstructionPlan>();

        public void CreateAConstructionPlan(int teamID, int planID, Vector3 pos)
        {
            var go =  PhotonNetwork.Instantiate("ConstructionPlan", pos, Quaternion.identity);
            var constructionPlan = go.GetComponentInChildren<ConstructionPlan>();
            constructionPlan.SetConstructionTeamID(teamID);
            _allConstructions.Add(constructionPlan);
        }
    }
}
