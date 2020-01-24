using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Construction
{
    public class ConstructionPiece : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();            
        }

        public void Construct()
        {
            _meshRenderer.material = (Material)Resources.Load("ConstructPieceMaterial");
        }

        public void SetMaterialColor(Color newColor)
        {
            _meshRenderer.material.SetColor("_MainColor", newColor);
        }
    }
}
