using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;
using Photon.Pun;

namespace Character
{
    public class Character3DUI : MonoBehaviourPun
    {
        [SerializeField] private TextMeshProUGUI _myNicknameText;
        private Vector3 _canvasOffset;
        [SerializeField] public Transform _myCharacter;
        [SerializeField] private GameObject _crown;

        private void Awake()
        {
            _canvasOffset = transform.position - _myCharacter.position;
        }

        private void LateUpdate()
        {
            transform.position = _myCharacter.position + _canvasOffset;
        }

        public void UpdateCrown(bool active)
        {
            _crown.SetActive(active);
        }

        [PunRPC]
        public void RPCUpdateCrown(bool active)
        {
            UpdateCrown(active);
        }

        public void UpdateNickname(string newNickname)
        {
            _myNicknameText.text = newNickname;
        }

        [PunRPC]
        public void RPCUpdateNickname(string actualNickname)
        {
            UpdateNickname(actualNickname);
        }
    }
}
