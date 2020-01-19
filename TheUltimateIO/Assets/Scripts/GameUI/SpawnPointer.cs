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

        public void UpdatePosition(Vector3 modelPos, float mapScale)
        {
            Vector2 finalAnchoredPos = new Vector2(modelPos.x / mapScale / 150f, modelPos.z / mapScale / 150f);
            GetComponent<RectTransform>().anchoredPosition = finalAnchoredPos;
        }

        public void SelectedButton()
        {
            modelSpawnPoint.ClickedOnSpawnPoint();
        }
    }
}
