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
        public List<int> allPlanPrices = new List<int>();
        private ConstructionPlan _preConstruction;
        private int _actualPlanID;
        private bool _canSpawn;
        public void CreateAPreConstructionPlan(int planID)
        {
            DestroyPreConstruction();
            _preConstruction = ((GameObject)Instantiate(Resources.Load(allPlans[planID]))).GetComponentInChildren<ConstructionPlan>();
            _preConstruction.enabled = false;
            _preConstruction.SetConstructionTeamID(6);
            _actualPlanID = planID;
            _preConstruction.ArtificialAwake();
        }

        public void DestroyPreConstruction()
        {
            if (_preConstruction != null)
                Destroy(_preConstruction.transform.parent.gameObject);
        }

        private Vector3 GetMouseSpawnPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Layers.FLOOR))
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
                    .RPC("RPCCreateAConstructionPlan", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _actualPlanID, _preConstruction.transform.parent.position);
            }
        }

        [PunRPC] public void RPCCreateAConstructionPlan(Player photonPlayer, int planID, Vector3 pos)
        {
            var go = PhotonNetwork.Instantiate(allPlans[planID], pos, Quaternion.identity);
            var constructionPlan = go.GetComponentInChildren<ConstructionPlan>();
            constructionPlan.photonView.RPC("RPCSetCollider", RpcTarget.AllBuffered);
            constructionPlan.photonView.RPC("RPCGeneralUpdate", RpcTarget.AllBuffered, allPlanPrices[planID], FindObjectOfType<Server>().allPlayers[photonPlayer].team);
            _allConstructions.Add(constructionPlan);
        }
    }
}
