using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity;

namespace Character
{
    public class CharacterCamera : IUpdatable
    {
        CharacterModel _myModel;
        Rigidbody _pelvisRb;
        private Action myAction = delegate { };
        public enum CameraMode
        {
            ThirdPersonMode = 0,
            GodMode = 1,
            CoreMode = 2
        }
        private CameraMode _actualCameraMode;
        private CameraMode _previousCameraMode;

        public CharacterCamera(CharacterModel model, Rigidbody pelvis)
        {
            _myModel = model;
            _pelvisRb = pelvis;
            model.OnChangeRespawnMode += ChangeRespawnMode;
            myAction = ThirdPersonCamera;
        }

        private void ChangeRespawnMode(CameraMode newCamMode)
        {
            if (newCamMode == _actualCameraMode) return;

            _previousCameraMode = _actualCameraMode;
            _actualCameraMode = newCamMode;
            switch (newCamMode)
            {
                case CameraMode.ThirdPersonMode:
                    myAction = ThirdPersonCamera;
                    break;
                case CameraMode.GodMode:
                    myAction = GodModeCamera;
                    break;
                case CameraMode.CoreMode:
                    myAction = CoreCamera;
                    break;
                default:
                    myAction = ThirdPersonCamera;
                    break;
            }
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

        private void CoreCamera()
        {
            if (_nearestCore == null) return;

            //Camera offset
            Vector3 camFwd = Camera.main.transform.forward;
            Camera.main.transform.position += camFwd * _myModel.coreDistancingSpeed * Time.deltaTime;
            Camera.main.transform.forward = Vector3.Lerp(camFwd,
                (_nearestCore.transform.position - Camera.main.transform.position).normalized, _myModel.coreDistancingSpeed * Time.deltaTime);
        }

        private Core _nearestCore;
        public void ArtificialLateUpdate() 
        {
            if (UnityEngine.Object.FindObjectsOfType<Core>().Count() < 1) return;

            var _nearestCore = UnityEngine.Object.FindObjectsOfType<Core>()
                .OrderBy(x => Vector3.Distance(_myModel.transform.position, x.transform.position)).First();
            if (_nearestCore == null) return;
            if (Vector3.Distance(_nearestCore.transform.position, _myModel.transform.position) > _myModel.coreDistancingCloseness)
            {
                if (_previousCameraMode == CameraMode.ThirdPersonMode)
                    ChangeRespawnMode(CameraMode.ThirdPersonMode);
            }
            else
            {
                if (_actualCameraMode == CameraMode.ThirdPersonMode)
                    ChangeRespawnMode(CameraMode.CoreMode);
            }
        }
    }
}
