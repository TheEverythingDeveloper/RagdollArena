using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using Photon;
using Photon.Realtime;
using Photon.Pun;

namespace GameUI
{
    public class SpawnPointer : MonoBehaviourPun
    {
        public SpawnPoint modelSpawnPoint;

        public void UpdatePosition(SpawnPoint model, float mapScale)
        {
            modelSpawnPoint = model;
            Vector3 modelPos = model.transform.position;
            Vector2 finalAnchoredPos = new Vector2(modelPos.x / mapScale, modelPos.z / mapScale);
            GetComponent<RectTransform>().anchoredPosition = finalAnchoredPos;
        }

        public void SelectedButton()
        {
            Debug.LogError("se toco boton para respawnear");
            modelSpawnPoint.ClickedOnSpawnPoint();
        }
    }
}
