using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Construction
{
    public class ConstructionPiece : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private ConstructionPlan _myPlan;
        public GameObject _prefabInstantiation;

        private void Awake() { _myPlan = transform.parent.GetComponentInChildren<ConstructionPlan>(); }

        public void Construct()
        {
            if(_prefabInstantiation != null)
            {
                _myPlan.ConstructPiece(_prefabInstantiation.name);
                return;
            }
            GetComponent<MeshRenderer>().material = (Material)Resources.Load("ConstructPieceMaterial");
            
        }

        public void SetMaterialColor(Color newColor)
        {
            GetComponent<MeshRenderer>().material.SetColor("_MainColor", newColor);
        }
    }
}
