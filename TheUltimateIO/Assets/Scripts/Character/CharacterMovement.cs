using UnityEngine;

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

        public CharacterMovement(CharacterModel model, Rigidbody pelvis, Quaternion initialRot)
        {
            _myModel = model;
            _pelvisRb = pelvis;
            _initialRot = initialRot;
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
            Debug.DrawLine(_myModel.transform.position,
                _myModel.transform.position + Vector3.down * _myModel.inAirDistance,
                _inAir ? Color.red : Color.green);
            _inAir = !Physics.Raycast(_pelvisRb.transform.position, Vector3.down, _myModel.inAirDistance, 1 << Layers.FLOOR);

            _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _pelvisRb.velocity.sqrMagnitude,
                _myModel.sqrMagnitudeInTimeSpeed * Time.deltaTime);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1 << Layers.FLOOR))
            {
                _direction = (hit.point - _pelvisRb.transform.position).normalized;

                _lookRotation = Quaternion.LookRotation(_direction);
                _lookRotation *= _initialRot;

                _pelvisRb.transform.localRotation = Quaternion.Slerp(
                    _pelvisRb.transform.localRotation, _lookRotation, Time.deltaTime * _myModel.rotationSpeed);
            }
        }

        public void ArtificialFixedUpdate()
        {
            var horAxis = Input.GetAxis("Horizontal") * _myModel.speed * Time.deltaTime;
            var verAxis = Input.GetAxis("Vertical") * _myModel.speed * Time.deltaTime;
            _pelvisRb.velocity = new Vector3(horAxis, _pelvisRb.velocity.y, verAxis);
        }

        public void ArtificialLateUpdate() { }
    }
}
