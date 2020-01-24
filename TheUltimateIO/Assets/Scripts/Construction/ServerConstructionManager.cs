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
        public List<string> allPlans = new List<string>();
        private ConstructionPlan _preConstruction;
        private int _actualPlanID;
        private bool _canSpawn;
        public void CreateAPreConstructionPlan(int planID)
        {
            DestroyPreConstruction();
            _preConstruction = ((GameObject)Instantiate(Resources.Load(allPlans[planID]))).GetComponentInChildren<ConstructionPlan>();
            _preConstruction.enabled = false;
            _actualPlanID = planID;
        }

        public void DestroyPreConstruction()
        {
            if (_preConstruction != null)
                Destroy(_preConstruction.gameObject);
        }

        [PunRPC] public void RPCCreateAConstructionPlan(Player photonPlayer, int planID, Vector3 pos)
        {
            var go =  PhotonNetwork.Instantiate("ConstructionPlan", pos, Quaternion.identity);
            var constructionPlan = go.GetComponentInChildren<ConstructionPlan>();
            constructionPlan.SetConstructionTeamID(FindObjectOfType<Server>().allPlayers[photonPlayer].team);
            _allConstructions.Add(constructionPlan);
            constructionPlan.enabled = true;
        }

        private Vector3 GetMouseSpawnPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
                _canSpawn = true;
            else
                _canSpawn = false;

            return hit.point;
        }

        private void Update()
        {
            if (_preConstruction == null) return;
            Vector3 hitPos = GetMouseSpawnPos();
            if (!_canSpawn) return;

            _preConstruction.transform.parent.transform.position = hitPos;

            if (Input.GetMouseButtonDown(0))
            {
                DestroyPreConstruction();
                FindObjectOfType<ServerConstructionManager>().photonView
                    .RPC("RPCCreateAConstructionPlan", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _actualPlanID, Vector3.zero);
            }
        }
    }
}
