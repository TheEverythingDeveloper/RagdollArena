﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using TMPro;
using Weapons;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Photon;

namespace Construction
{
    public class ConstructionPanel : MonoBehaviour
    {
        public int actualConstructionSelection;
        private bool _openedPanel;
        private bool _constructionSelected;
        [SerializeField] private GameObject _constructionPanelGo;
        private WeaponsAndStatsUIManager _weaponsAndStats;
        private Image[] _allConstructionOptions;
        public event Action<bool> OnConstructionMode = delegate { };

        private void Awake()
        {
            _weaponsAndStats = FindObjectOfType<WeaponsAndStatsUIManager>();
            _allConstructionOptions = GetComponentsInChildren<Button>().Select(x => x.GetComponent<Image>()).ToArray();
            OpenClosePanel(false);
        }

        public void OpenClosePanel(bool open)
        {
            _openedPanel = open;
            _constructionPanelGo.SetActive(open);
            _weaponsAndStats.gameObject.SetActive(!open);
            OnConstructionMode(open);
        }

        public void ConstructionSelection(int ID)
        {
            foreach (var x in _allConstructionOptions)
                x.color = Color.white;
            actualConstructionSelection = ID;
            _allConstructionOptions[ID].color = Color.green;
            _constructionSelected = true;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if (_openedPanel)
                    OpenClosePanel(false);
                else
                    OpenClosePanel(true);
            }
            if (Input.GetMouseButtonDown(1))
                _constructionSelected = false;

            if (_constructionSelected)
            {
                FindObjectOfType<ServerConstructionManager>().CreateAPreConstructionPlan(actualConstructionSelection);
                _constructionSelected = false;
            }
        }
    }
}
