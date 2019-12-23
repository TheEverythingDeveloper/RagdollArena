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
            _myModel.OnCrowned += OnCrowned;
        }

        public void OnCrowned(bool active)
        {
            _3dUI.photonView.RPC("RPCUpdateCrown", Photon.Pun.RpcTarget.AllBuffered, active);
        }

        public void ArtificialAwake() { }

        public void ArtificialStart() { }

        public void ArtificialUpdate() { }

        public void ArtificialFixedUpdate() { }

        public void ArtificialLateUpdate() { }
    }
}
