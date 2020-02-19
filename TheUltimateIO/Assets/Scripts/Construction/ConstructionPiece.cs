using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
    public class ConstructionPiece : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private ConstructionPlan _myPlan;
        public GameObject _prefabInstantiation;

        private void Awake() { _myPlan = GetComponentInParent<ConstructionPlan>(); }

        public void Construct()
        {
            if(_prefabInstantiation != null)
            {
                var go = Instantiate(_prefabInstantiation, transform.parent.position, transform.parent.rotation);
                //TODO: Aca se llama lo que se necesite setear del ariete/catapulta/etc, por ejemplo, cambio de team para actualizar el color
                //Cualquier dato que se necesite se consigue del constructionplan, y no de aca. Por ejemplo, para conseguir el team es _myPlan.team;
                Destroy(gameObject);
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
