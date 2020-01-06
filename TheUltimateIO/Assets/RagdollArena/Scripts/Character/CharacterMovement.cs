using UnityEngine;
using System;

namespace Character
{
    public class CharacterMovement : IUpdatable, IConstructable
    {
        CharacterModel _myModel;
        Rigidbody _pelvisRb;
        bool _inAir;
        public bool inAir { get { return _inAir; } }
        Quaternion _initialRot;
        Quaternion _lookRotation;
        Vector3 _direction;
        float _sqrMagnitudeInTime;
        private LayerMask _floorLayers;
        Action updateControls;
        Action<float, float> fixedUpdateControls;

        public CharacterMovement(CharacterModel model, Rigidbody pelvis, Quaternion initialRot, LayerMask floorLayers)
        {
            _floorLayers = floorLayers;
            _myModel = model;
            _pelvisRb = pelvis;
            _initialRot = initialRot;
            ChangeControls(true);
        }

        public void ArtificialAwake()
        {
            _myModel.OnJump += Jump;
        }

        public void ArtificialStart() { }

        public void Jump()
        {
            _inAir = true;
            _pelvisRb.AddForce(Vector3.up * _myModel.jumpSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        public void ArtificialUpdate()
        {
            updateControls();
        }

        public void ArtificialFixedUpdate()
        {
            //fixedUpdateControls();
        }

        public void ChangeControls(bool normal)
        {
            if (normal)
            {
                updateControls = NormalUpdateControls;
                fixedUpdateControls = NormalFixedUpdateControls;
            }
            else
            {
                updateControls = DrunkUpdateControls;
                fixedUpdateControls = DrunkFixedUpdateControls;
            }
        }
        #region NormalControls
        void NormalUpdateControls()
        {
            Debug.DrawLine(_myModel.transform.position,
            _myModel.transform.position + Vector3.down * _myModel.inAirDistance,
            _inAir ? Color.red : Color.green);
            _inAir = !Physics.Raycast(_pelvisRb.transform.position, Vector3.down, _myModel.inAirDistance, _floorLayers);

            _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _pelvisRb.velocity.sqrMagnitude,
                _myModel.sqrMagnitudeInTimeSpeed * Time.deltaTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayers))
            {
                if (Vector3.Distance(hit.point, _pelvisRb.transform.position) < 2) return;
                _direction = (hit.point - _pelvisRb.transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                _lookRotation *= _initialRot;

                _pelvisRb.transform.localRotation = Quaternion.Slerp(
                    _pelvisRb.transform.localRotation, _lookRotation, Time.deltaTime * _myModel.rotationSpeed);

                _pelvisRb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, _lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
            }
        }

        public void Move(float horizontal, float vertical)
        {
            fixedUpdateControls(horizontal, vertical);
        }

        void NormalFixedUpdateControls(float horizontal, float vertical)
        {
            var horAxis = horizontal * _myModel.speed * Time.deltaTime;
            var verAxis = vertical * _myModel.speed * Time.deltaTime;

            /*var animMove = horAxis != 0 || verAxis != 0;
            if (animMove != _myModel.anim.GetBool("Move")) _myModel.anim.SetBool("Move", animMove);*/

            _myModel.transform.position -= _myModel.pelvisRb.transform.forward * horAxis;
            _myModel.transform.position += _myModel.pelvisRb.transform.up * verAxis;
            //_pelvisRb.velocity = new Vector3(horAxis, _pelvisRb.velocity.y, verAxis);
        }
        #endregion
        void DrunkUpdateControls()
        {
            _inAir = !Physics.Raycast(_pelvisRb.transform.position, Vector3.down, _myModel.inAirDistance, _floorLayers);

            _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _pelvisRb.velocity.sqrMagnitude,
                _myModel.sqrMagnitudeInTimeSpeed * Time.deltaTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayers))
            {
                if (Vector3.Distance(hit.point, _pelvisRb.transform.position) < 2) return;
                _direction = (hit.point - _pelvisRb.transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                _lookRotation *= _initialRot;

                _pelvisRb.transform.localRotation = Quaternion.Slerp(
                    _pelvisRb.transform.localRotation, _lookRotation, Time.deltaTime * _myModel.rotationSpeed);

                _pelvisRb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, -_lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
            }
        }

        void DrunkFixedUpdateControls(float horizontal, float vertical)
        {
            var horAxis = vertical * _myModel.speed * Time.deltaTime;
            var verAxis = horizontal * _myModel.speed * Time.deltaTime;

            /*var animMove = horAxis != 0 || verAxis != 0;
            if (animMove != _myModel.anim.GetBool("Move")) _myModel.anim.SetBool("Move", animMove);*/

            _myModel.transform.position -= _myModel.pelvisRb.transform.forward * horAxis;
            _myModel.transform.position += _myModel.pelvisRb.transform.up * verAxis;
        }

        public void ArtificialLateUpdate() { }
    }
}
