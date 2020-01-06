using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterView : IConstructable, IUpdatable
    {
        CharacterModel _myModel;
        Character3DUI _3dUI;
        public CharacterView(CharacterModel model)
        {
            _myModel = model;
            _3dUI = model.transform.parent.GetComponentInChildren<Character3DUI>();
            _myModel.name = PhotonNetwork.NickName;
            _3dUI.photonView.RPC("RPCUpdateNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
            _myModel.OnCrowned += OnCrowned;
            _myModel.OnChangeRespawnMode += OnChangeRespawnMode;
        }

        public void OnCrowned(bool active)
        {
            _3dUI.photonView.RPC("RPCUpdateCrown", Photon.Pun.RpcTarget.AllBuffered, active);
        }

        public void OnChangeRespawnMode(bool active)
        {
            _3dUI.gameObject.SetActive(active);
        }

        public void ArtificialAwake() { }
        public void ArtificialStart() { }
        public void ArtificialUpdate() { }
        public void ArtificialFixedUpdate() { }
        public void ArtificialLateUpdate() { }
    }
}
