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
        [SerializeField] private Transform _myCharacter;

        private void Awake()
        {
            _canvasOffset = transform.position - _myCharacter.position;
        }

        private void LateUpdate()
        {
            transform.position = _myCharacter.position + _canvasOffset;
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
