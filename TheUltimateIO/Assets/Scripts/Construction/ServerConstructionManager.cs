using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Construction
{
    public class ServerConstructionManager : MonoBehaviourPun
    {
        public List<ConstructionPlan> _allConstructions = new List<ConstructionPlan>();

        [PunRPC] public void RPCCreateAConstructionPlan(Player photonPlayer, int planID, Vector3 pos)
        {
            var go =  PhotonNetwork.Instantiate("ConstructionPlan", pos, Quaternion.identity);
            var constructionPlan = go.GetComponentInChildren<ConstructionPlan>();
            constructionPlan.SetConstructionTeamID(FindObjectOfType<Server>().allPlayers[photonPlayer].team);
            _allConstructions.Add(constructionPlan);
        }
    }
}
