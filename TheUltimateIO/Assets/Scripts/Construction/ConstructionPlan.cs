using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Character;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Weapons;
using Photon.Realtime;
using Photon.Pun;
using Photon;

namespace Construction
{
    public class ConstructionPlan : MonoBehaviourPun
    {
        public int totalBlocks;
        public int actualBlocks;
        public int constructionTeamID;
        private bool _playerIn;
        [HideInInspector] public ConstructionPiece constructionPiece;
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _completionImg;

        public void SetConstructionTeamID(int teamID)
        {
            constructionTeamID = teamID;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Layers.PLAYER)
            {
                CharacterModel otherModel = other.gameObject.GetComponentInParent<CharacterModel>();
                if (otherModel == null || otherModel.team != constructionTeamID) return; //si no es del team o no era un model entonces return

                PlayerActivation(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == Layers.PLAYER)
            {
                CharacterModel otherModel = other.gameObject.GetComponentInParent<CharacterModel>();
                if (otherModel == null || otherModel.team != constructionTeamID) return; //si no es del team o no era un model entonces return

                PlayerActivation(false);
            }
        }

        public void SetPlanName(string newName) => _nameText.text = newName;
        public void SetProgress(float amount01, int actual, int total)
        {
            _completionImg.fillAmount = amount01;
            _amountText.text = actual + " / " + total;
        }

        public void PlayerActivation(bool enter)
        {
            _infoPanel.SetActive(enter);
        }

        private void OnMouseEnter()
        {
            if (!_playerIn) return;
           //TODO: Feedback de poner mouse arriba
        }

        private void OnMouseExit()
        {
            if (!_playerIn) return;
            //TODO: Sacar el feedback de poner mouse arriba
        }

        private void OnMouseUp()
        {
            if (!_playerIn) return;
            Debug.Log("Se dono bloques al plan de construccion");

            //TODO: Llamar a un RPC del server que le pregunte al model para ver si tiene bloques para donar. Por cada click le va a sumar uno.
        }

        public void AddBlocks(int newBlocks) => photonView.RPC("RPCAddBlocks", RpcTarget.All, newBlocks);
        [PunRPC] public void RPCAddBlocks(int newBlocks)
        {
            actualBlocks += newBlocks;

            //TODO: 3DHud del plan actualizado con la cantidad de bloques que tiene ahora
            //TODO: Feedback de que pusiste bloques, particulas, sonido
            //TODO: Algun tipo de shader que se vaya llenando en el modelo como feedback 3D.

            if (actualBlocks >= totalBlocks)
                Construct();
        }

        public void Construct()
        {
            constructionPiece.Construct();
        }
    }
}
