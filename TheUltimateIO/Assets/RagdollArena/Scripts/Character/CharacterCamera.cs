using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterCamera : IUpdatable
    {
        CharacterModel _myModel;
        Rigidbody _pelvisRb;
        private Action myAction = delegate { };

        public CharacterCamera(CharacterModel model, Rigidbody pelvis)
        {
            _myModel = model;
            _pelvisRb = pelvis;
            model.OnChangeRespawnMode += ChangeRespawnMode;
            myAction = ThirdPersonCamera;
        }

        private void ChangeRespawnMode(bool dead)
        {
            if (!dead)
                myAction = GodModeCamera;
            else
                myAction = ThirdPersonCamera;
        }

        public void ArtificialUpdate() { }
        public void ArtificialFixedUpdate()
        {
            myAction();
        }

        private void ThirdPersonCamera()
        {
            //FoV Camera
            Camera.main.fieldOfView = Mathf.Lerp(_myModel.minFOV, _myModel.maxFOV,
                _pelvisRb.velocity.sqrMagnitude * _myModel.ratioMultiplierFoV);

            //Camera offset
            Vector3 offsetPosition = _myModel.pelvisRb.transform.position + _myModel.thirdPersonCameraOffset;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                offsetPosition, _myModel.cameraSpeed * Time.deltaTime);
        }

        private void GodModeCamera()
        {
            //FoV Camera
            Camera.main.fieldOfView = Mathf.Lerp(_myModel.minFOV, _myModel.maxFOV,
                _pelvisRb.velocity.sqrMagnitude * _myModel.ratioMultiplierFoV);

            //Camera offset
            Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward,
                _myModel.godModeCameraOffset.forward, _myModel.cameraSpeed / 10f * Time.deltaTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                _myModel.godModeCameraOffset.position, _myModel.cameraSpeed / 7f * Time.deltaTime);
        }

        public void ArtificialLateUpdate() { }
    }
}
