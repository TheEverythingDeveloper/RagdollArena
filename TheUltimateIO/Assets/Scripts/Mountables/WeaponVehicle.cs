using UnityEngine;
using Photon.Pun;
public class WeaponVehicle : MonoBehaviourPun
{
    public float limit;
    public float speedAttack, speedReturn;
    public float damage;

    protected bool _attack;
    protected Coroutine _coroutineAttack;
    protected Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.localPosition;
    }

    public virtual void Attack()
    {

    }

    public virtual void WeaponActive()
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {

    }
}
