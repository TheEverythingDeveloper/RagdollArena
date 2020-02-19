using UnityEngine;
using System;

namespace Character
{
    public class CharacterMovement : IUpdatable, IConstructable
    {
        CharacterModel _myModel;
        Rigidbody _rb;
        bool _inAir;
        public bool inAir { get { return _inAir; } }
        Quaternion _initialRot;
        Quaternion _lookRotation;
        Vector3 _direction;
        float _sqrMagnitudeInTime;
        private LayerMask _floorLayers;
        Action updateControls;
        Action<float, float> fixedUpdateControls;

        public CharacterMovement(CharacterModel model, Rigidbody rigidbody, Quaternion initialRot, LayerMask floorLayers)
        {
            _floorLayers = floorLayers;
            _myModel = model;
            _rb = rigidbody;
            _initialRot = initialRot;
            ChangeControls(true);
        }

        public void ArtificialAwake()
        {
            //_myModel.OnJump += Jump;
        }

        public void ArtificialStart() { }

        public void Jump()
        {
            _inAir = true;
            _rb.AddForce(Vector3.up * _myModel.characterStats.jumpSpeed, ForceMode.Impulse);
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
            _inAir = !Physics.Raycast(_rb.transform.position, Vector3.down, _myModel.inAirDistance, _floorLayers);

            _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _rb.velocity.sqrMagnitude,
                _myModel.sqrMagnitudeInTimeSpeed * Time.deltaTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayers))
            {
                if (Vector3.Distance(hit.point, _rb.transform.position) < 2) return;
                _direction = (hit.point - _rb.transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                _lookRotation *= _initialRot;

                _rb.transform.localRotation = Quaternion.Slerp(
                    _rb.transform.localRotation, _lookRotation, Time.deltaTime * _myModel.characterStats.rotationSpeed);

                _rb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, _lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
            }
        }

        public void Move(float horizontal, float vertical) { fixedUpdateControls(horizontal, vertical); }

        void NormalFixedUpdateControls(float horizontal, float vertical)
        {
            var horAxis = horizontal * _myModel.characterStats.speed * Time.deltaTime;
            var verAxis = vertical * _myModel.characterStats.speed * Time.deltaTime;

            _myModel.anim.SetTrigger("Walk");

            var dir = new Vector3(horAxis, 0, verAxis);
            _rb.transform.position += dir;
        }
        #endregion
        void DrunkUpdateControls()
        {
            _inAir = !Physics.Raycast(_rb.transform.position, Vector3.down, _myModel.inAirDistance, _floorLayers);

            _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _rb.velocity.sqrMagnitude,
                _myModel.sqrMagnitudeInTimeSpeed * Time.deltaTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _floorLayers))
            {
                if (Vector3.Distance(hit.point, _rb.transform.position) < 2) return;
                _direction = (hit.point - _rb.transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(_direction);
                _lookRotation *= _initialRot;

                _rb.transform.localRotation = Quaternion.Slerp(
                    _rb.transform.localRotation, _lookRotation, Time.deltaTime * _myModel.characterStats.rotationSpeed);

                _rb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, -_lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
            }
        }

        void DrunkFixedUpdateControls(float horizontal, float vertical)
        {
            var horAxis = vertical * _myModel.characterStats.speed * Time.deltaTime;
            var verAxis = horizontal * _myModel.characterStats.speed * Time.deltaTime;

            /*var animMove = horAxis != 0 || verAxis != 0;
            if (animMove != _myModel.anim.GetBool("Move")) _myModel.anim.SetBool("Move", animMove);*/

            var dir = new Vector3(horAxis, 0, verAxis);
            _rb.transform.position += dir;
        }

        public void ArtificialLateUpdate() { }
    }
}
