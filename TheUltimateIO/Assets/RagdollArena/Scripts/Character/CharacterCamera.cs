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
        public Transform catapultLook;
        Quaternion _initialRot;
        CharacterModel _myModel;
        Rigidbody rb;
        private Action myAction = delegate { };
        public enum CameraMode
        {
            ThirdPersonMode = 0,
            GodMode = 1,
            CoreMode = 2,
            CatapultMode = 3
        }
        private CameraMode _actualCameraMode;
        private CameraMode _previousCameraMode;

        public CharacterCamera(CharacterModel model, Rigidbody pelvis)
        {
            _myModel = model;
            rb = pelvis;
            model.OnChangeRespawnMode += ChangeRespawnMode;
            _initialRot = Camera.main.transform.rotation;
            myAction = ThirdPersonCamera;
        }

        public void ChangeTarget(Rigidbody newRb)
        {
            rb = newRb;
        }

        public void ChangeRespawnMode(CameraMode newCamMode)
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
                case CameraMode.CatapultMode:
                    myAction = CatapultCamera;
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
                rb.velocity.sqrMagnitude * _myModel.ratioMultiplierFoV);

            //Camera offset
            Vector3 offsetPosition = rb.transform.position + _myModel.thirdPersonCameraOffset;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                offsetPosition, _myModel.cameraSpeed * Time.deltaTime);

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, _initialRot, _myModel.cameraSpeed * Time.deltaTime);
        }

        private void GodModeCamera()
        {
            //FoV Camera
            Camera.main.fieldOfView = Mathf.Lerp(_myModel.minFOV, _myModel.maxFOV,
                rb.velocity.sqrMagnitude * _myModel.ratioMultiplierFoV);

            //Camera offset
            Camera.main.transform.forward = Vector3.Lerp(Camera.main.transform.forward,
                _myModel.godModeCameraOffset.forward, _myModel.cameraSpeed / 10f * Time.deltaTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                _myModel.godModeCameraOffset.position, _myModel.cameraSpeed / 7f * Time.deltaTime);

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, _initialRot, _myModel.cameraSpeed * Time.deltaTime);
        }

        private void CoreCamera()
        {
            if (_nearestCore == null) return;

            Vector3 offsetPosition = _nearestCore.transform.position + _myModel.thirdPersonCameraOffset * 2 - Vector3.forward * 4;
            Vector3 resultVector = Vector3.Lerp(Camera.main.transform.position,
                offsetPosition, _myModel.coreDistancingSpeed * Time.deltaTime);
            Camera.main.transform.position = resultVector;

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, _initialRot, _myModel.cameraSpeed * Time.deltaTime);
        }

        private void CatapultCamera()
        {
            if (catapultLook == null) return;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position,
                catapultLook.position, _myModel.cameraSpeed * Time.deltaTime);

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, catapultLook.rotation, _myModel.cameraSpeed * Time.deltaTime);
        }

        private Core _nearestCore;
        public void ArtificialLateUpdate() 
        {
            if (UnityEngine.Object.FindObjectsOfType<Core>().Count() < 1) return;

            _nearestCore = UnityEngine.Object.FindObjectsOfType<Core>()
                .OrderBy(x => Vector3.Distance(rb.transform.position, x.transform.position)).First();
            if (_nearestCore == null) return;
            if (Vector3.Distance(_nearestCore.transform.position, rb.transform.position) > _myModel.coreDistancingCloseness)
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
