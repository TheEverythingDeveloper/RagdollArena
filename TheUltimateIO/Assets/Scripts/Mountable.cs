using UnityEngine;
using Character;
using Photon.Pun;

public class Mountable : MonoBehaviourPun
{
    public CharacterModel _characterModel;
    public float sqrMagnitudeInTimeSpeed;
    public float rotationSpeed;
    public LayerMask floorLayers;
    protected Rigidbody _rb;

    protected Quaternion _initialRot;
    protected Quaternion _lookRotation;
    protected Vector3 _direction;
    protected float _sqrMagnitudeInTime;

    public virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();      
    }

    public virtual void ActiveMountable()
    {
        _characterModel.ChangeControls(ArtificialUpdate, ArtificialFixedUpdate, ArtificialLateUpdate, Move, _rb);
    }

    public virtual void HideModelCharacter(bool hide)
    {
        _characterModel.HideModel(hide);
    }

    public virtual void ArtificialUpdate()
    {

    }

    public virtual void ArtificialFixedUpdate()
    {
        
    }

    public virtual void ArtificialLateUpdate()
    {
        
    }

    public virtual void Move(float horizontal, float vertical)
    {

    }

    public virtual void EnterMountable()
    {

    }

    public virtual void ExitMountable()
    {

    }

    public virtual void RotateLookMouse()
    {
        _sqrMagnitudeInTime = Mathf.Lerp(_sqrMagnitudeInTime, _rb.velocity.sqrMagnitude,
            sqrMagnitudeInTimeSpeed * Time.deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayers))
        {
            if (Vector3.Distance(hit.point, _rb.transform.position) < 2) return;
            _direction = (hit.point - _rb.transform.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);
            _lookRotation *= _initialRot;

            _rb.transform.localRotation = Quaternion.Slerp(
                _rb.transform.localRotation, _lookRotation, Time.deltaTime * rotationSpeed);

            _rb.transform.localRotation = Quaternion.Euler(_initialRot.eulerAngles.x, _lookRotation.eulerAngles.y, _initialRot.eulerAngles.z);
        }
    }
}
