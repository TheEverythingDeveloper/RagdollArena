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
        private int _totalBlocks; public int TotalBlocks
        {
            get { return _totalBlocks; }
            set { _totalBlocks = Mathf.Max(0, value); }
        }
        private int _actualBlocks; public int ActualBlocks
        {
            get { return _actualBlocks; }
            set { _actualBlocks = Mathf.Clamp(value, 0, TotalBlocks); }
        }
        public int teamID;
        private bool _playerIn;
        private bool _constructed;
        private bool _prePlan;
        [HideInInspector] public ConstructionPiece constructionPiece;
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _nameText;

        public void SetConstructionTeamID(int teamID)
        {
            this.teamID = teamID;
        }

        private void Awake()
        {
            constructionPiece = transform.parent.GetComponentInChildren<ConstructionPiece>();
            TotalBlocks = 100;
            ActualBlocks = 0;
            SetProgress(0,100);
            constructionPiece.SetMaterialColor(teamID == 0 ? Color.blue : teamID == 1 ? Color.red : teamID == 2 ? Color.yellow : Color.green);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_constructed) return;

            if (other.gameObject.layer == Layers.PLAYER)
            {
                CharacterModel otherModel = other.gameObject.GetComponentInParent<CharacterModel>();
                if (otherModel == null || otherModel.team != teamID || !otherModel.owned) return; //si no es del team o no era un model entonces return

                PlayerActivation(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_constructed) return;

            if (other.gameObject.layer == Layers.PLAYER)
            {
                CharacterModel otherModel = other.gameObject.GetComponentInParent<CharacterModel>();
                if (otherModel == null || otherModel.team != teamID || !otherModel.owned) return; //si no es del team o no era un model entonces return

                PlayerActivation(false);
            }
        }

        public void SetPlanName(string newName) => _nameText.text = newName;
        public void SetProgress(int actual, int total)
        {
            TotalBlocks = total;
            ActualBlocks = actual;
            _amountText.text = actual + " / " + total;
        }

        public void PlayerActivation(bool enter)
        {
            if (_constructed) return;

            _infoPanel.SetActive(enter);
            _playerIn = enter;
        }

        private void OnMouseEnter()
        {
            if (!_playerIn || _constructed) return;
           //TODO: Feedback de poner mouse arriba
        }

        private void OnMouseExit()
        {
            if (!_playerIn || _constructed) return;
            //TODO: Sacar el feedback de poner mouse arriba
        }
        
        private void Update()
        {
            if (!_playerIn || _constructed) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
                {
                    if (hit.collider.gameObject.GetComponentInChildren<ConstructionPiece>())
                    {
                        Debug.Log("Se dono a la construccion");
                        AddBlocks(10); //En vez de esto, hay que hacer que lo haga desde el server habiendo preguntado por los bloques del player
                    }
                    //TODO: Llamar a un RPC del server que le pregunte al model para ver si tiene bloques para donar. Por cada click le va a sumar uno.
                }
            }
        }

        public void AddBlocks(int newBlocks) => photonView.RPC("RPCAddBlocks", RpcTarget.All, newBlocks);
        [PunRPC] public void RPCAddBlocks(int newBlocks)
        {
            ActualBlocks += newBlocks;

            SetProgress(_actualBlocks, TotalBlocks);

            //TODO: 3DHud del plan actualizado con la cantidad de bloques que tiene ahora
            //TODO: Feedback de que pusiste bloques, particulas, sonido
            //TODO: Algun tipo de shader que se vaya llenando en el modelo como feedback 3D.

            if (ActualBlocks >= TotalBlocks)
                Construct();
        }

        public void Construct()
        {
            _constructed = true;
            constructionPiece.Construct();
            _amountText.gameObject.SetActive(false);
        }
    }
}
