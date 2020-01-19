using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI
{
    public class SpawnMapArea : MonoBehaviour, IPointerClickHandler
    {
        private SpawnMap _spawnMap;

        private void Awake() => _spawnMap = FindObjectOfType<SpawnMap>();

        public void OnPointerClick(PointerEventData eventData)
        {
            //_spawnMap.PointerClick();
        }
    }
}
