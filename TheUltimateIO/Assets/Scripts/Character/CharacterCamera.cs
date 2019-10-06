using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterCamera : IUpdatable
    {
        CharacterModel _myModel;
        Rigidbody _pelvisRb;

        public CharacterCamera(CharacterModel model, Rigidbody pelvis)
        {
            _myModel = model;
            _pelvisRb = pelvis;
        }

        public void ArtificialUpdate() { }
        public void ArtificialFixedUpdate()
        {
            //FoV Camera
            Camera.main.fieldOfView = Mathf.Lerp(_myModel.minFOV, _myModel.maxFOV,
                _pelvisRb.velocity.sqrMagnitude * _myModel.ratioMultiplierFoV);

            //Camera offset
            Vector3 offsetPosition = _myModel.pelvisRb.transform.position + _myModel.cameraOffset;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                offsetPosition, _myModel.cameraSpeed * Time.deltaTime);
        }
        public void ArtificialLateUpdate() { }
    }
}
