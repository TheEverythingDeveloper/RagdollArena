using UnityEngine;

public class WeaponVehicle : MonoBehaviour
{
    public float maxPosZ;
    public float speedAttack, speedReturn;
    public float damage;

    protected bool _attack;
    protected Coroutine _coroutineAttack;
    protected Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.localPosition;
    }

    public virtual void ActiveWeapon()
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {

    }
}
