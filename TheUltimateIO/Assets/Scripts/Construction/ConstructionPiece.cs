using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
    public class ConstructionPiece : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        public void Construct()
        {
            GetComponent<MeshRenderer>().material = (Material)Resources.Load("ConstructPieceMaterial");
        }

        public void SetMaterialColor(Color newColor)
        {
            GetComponent<MeshRenderer>().material.SetColor("_MainColor", newColor);
        }
    }
}
